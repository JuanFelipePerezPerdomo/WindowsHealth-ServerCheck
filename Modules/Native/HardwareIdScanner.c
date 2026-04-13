/*
 * HardwareIdScanner.c
 *
 * DLL en C puro que enumera dispositivos y sus Hardware IDs usando SetupAPI.
 * Diseñada para ser importada desde C# via P/Invoke.
 *
 * Compilar con:
 *   cl.exe HardwareIdScanner.c /LD /O2 /W4
 *       /link setupapi.lib cfgmgr32.lib kernel32.lib
 *
 * O con MinGW:
 *   gcc HardwareIdScanner.c -shared -o HardwareIdScanner.dll
 *       -lsetupapi -lcfgmgr32 -lkernel32 -O2 -Wall
 */

#define WIN32_LEAN_AND_MEAN
#include <windows.h>
#include <setupapi.h>
#include <cfgmgr32.h>  
#include <devguid.h>
#include <regstr.h>
#include <stdio.h>
#include <stdlib.h>
#include <string.h>

#pragma comment(lib, "setupapi.lib")
#pragma comment(lib, "cfgmgr32.lib")

 /*
  *  Estructuras públicas
  */

  /*
   * Representa un único Hardware ID de un dispositivo.
   * Un dispositivo puede tener varios (del más específico al más genérico).
   */
typedef struct {
    wchar_t hardwareId[512];   /* p.ej. "PCI\VEN_8086&DEV_9A3D&SUBSYS_..." */
    wchar_t compatibleId[512]; /* ID compatible (más genérico) */
} HardwareIdEntry;

/*
 * Resultado completo de un dispositivo enumerado.
 */
typedef struct {
    wchar_t instanceId[512];        /* ID de instancia único en el sistema   */
    wchar_t deviceDescription[256]; /* Nombre legible del dispositivo        */
    wchar_t manufacturer[128];      /* Fabricante registrado                 */
    wchar_t driverVersion[64];      /* Versión del driver instalado          */
    wchar_t driverDate[32];         /* Fecha del driver (YYYY-MM-DD)         */
    wchar_t infPath[MAX_PATH];      /* Ruta al .inf que instaló el driver    */
    wchar_t classGuid[64];          /* GUID de la clase del dispositivo      */
    int     hardwareIdCount;        /* Cuántas entradas hay en hardwareIds[] */
    HardwareIdEntry hardwareIds[8]; /* Hasta 8 IDs (suficiente en práctica)  */
    int     hasDriver;              /* 1 si tiene driver instalado           */
    int     isProblem;              /* 1 si el dispositivo reporta un error  */
    DWORD   problemCode;            /* Código CM_PROB_* si isProblem == 1    */
} DeviceRecord;

/*
 * Colección de resultados devuelta al caller.
 * El caller DEBE liberar con FreeDeviceList().
 */
typedef struct {
    DeviceRecord* records;  /* Array asignado con HeapAlloc */
    int           count;    /* Número de elementos válidos  */
    int           capacity; /* Capacidad actual del array   */
} DeviceList;

/*
 *  Helpers internos — sin allocaciones globales
*/

/*
 * Lee una propiedad de cadena del registro del dispositivo.
 * Devuelve 1 si tuvo éxito, 0 si no.
 * dst debe tener al menos dstLen wchar_t de espacio.
 */
static int ReadStringProperty(
    HDEVINFO        devInfo,
    SP_DEVINFO_DATA* devData,
    DWORD           property,
    wchar_t* dst,
    DWORD           dstLen)
{
    DWORD dataType = 0;
    DWORD required = 0;
    BOOL ok = SetupDiGetDeviceRegistryPropertyW(
        devInfo, devData, property,
        &dataType, (PBYTE)dst, dstLen * sizeof(wchar_t), &required);
    if (!ok || dataType != REG_SZ) {
        dst[0] = L'\0';
        return 0;
    }
    return 1;
}

/*
 * Lee todos los Hardware IDs de un dispositivo (REG_MULTI_SZ).
 * Rellena entry[] y devuelve cuántos se leyeron (máximo maxEntries).
 */
static int ReadHardwareIds(
    HDEVINFO         devInfo,
    SP_DEVINFO_DATA* devData,
    HardwareIdEntry* entries,
    int              maxEntries)
{
    /* Buffer temporal en el heap para evitar stack overflow con listas largas */
    const DWORD bufSize = 8192;
    wchar_t* buf = (wchar_t*)HeapAlloc(GetProcessHeap(), HEAP_ZERO_MEMORY,
        bufSize * sizeof(wchar_t));
    if (!buf) return 0;

    int count = 0;
    DWORD dataType = 0;
    DWORD required = 0;

    /* SPDRP_HARDWAREID devuelve REG_MULTI_SZ: strings terminadas en \0
       con un \0 adicional al final del bloque completo */
    BOOL ok = SetupDiGetDeviceRegistryPropertyW(
        devInfo, devData, SPDRP_HARDWAREID,
        &dataType, (PBYTE)buf, bufSize * sizeof(wchar_t), &required);

    if (ok && dataType == REG_MULTI_SZ) {
        wchar_t* p = buf;
        while (*p != L'\0' && count < maxEntries) {
            wcsncpy_s(entries[count].hardwareId,
                sizeof(entries[count].hardwareId) / sizeof(wchar_t),
                p, _TRUNCATE);
            entries[count].compatibleId[0] = L'\0'; /* se rellena después */
            count++;
            p += wcslen(p) + 1; /* avanzar al siguiente string */
        }
    }

    /* SPDRP_COMPATIBLEIDS: IDs compatibles (más genéricos) */
    memset(buf, 0, bufSize * sizeof(wchar_t));
    ok = SetupDiGetDeviceRegistryPropertyW(
        devInfo, devData, SPDRP_COMPATIBLEIDS,
        &dataType, (PBYTE)buf, bufSize * sizeof(wchar_t), &required);

    if (ok && dataType == REG_MULTI_SZ) {
        wchar_t* p = buf;
        int idx = 0;
        while (*p != L'\0' && idx < maxEntries) {
            if (idx < count) {
                /* asociar compatible ID al mismo índice que el hardware ID */
                wcsncpy_s(entries[idx].compatibleId,
                    sizeof(entries[idx].compatibleId) / sizeof(wchar_t),
                    p, _TRUNCATE);
            }
            idx++;
            p += wcslen(p) + 1;
        }
    }

    HeapFree(GetProcessHeap(), 0, buf);
    return count;
}

/*
 * Lee la versión y fecha del driver desde el sub-árbol de driver del dispositivo.
 * Accede directamente a HKLM\SYSTEM\CurrentControlSet\Control\Class\{GUID}\NNNN
 */
static void ReadDriverVersionInfo(
    HDEVINFO         devInfo,
    SP_DEVINFO_DATA* devData,
    wchar_t* outVersion,
    DWORD            versionLen,
    wchar_t* outDate,
    DWORD            dateLen,
    wchar_t* outInfPath,
    DWORD            infPathLen)
{
    outVersion[0] = L'\0';
    outDate[0] = L'\0';
    outInfPath[0] = L'\0';

    /* Abrir la clave de driver del dispositivo */
    HKEY hKey = SetupDiOpenDevRegKey(
        devInfo, devData,
        DICS_FLAG_GLOBAL, 0,
        DIREG_DRV,        /* clave de driver, no de dispositivo */
        KEY_READ);

    if (hKey == INVALID_HANDLE_VALUE) return;

    DWORD type = 0, size = 0;

    /* DriverVersion */
    size = versionLen * sizeof(wchar_t);
    RegQueryValueExW(hKey, L"DriverVersion", NULL, &type,
        (LPBYTE)outVersion, &size);

    /* DriverDate */
    size = dateLen * sizeof(wchar_t);
    RegQueryValueExW(hKey, L"DriverDate", NULL, &type,
        (LPBYTE)outDate, &size);

    /* InfPath (ruta al .inf) */
    size = infPathLen * sizeof(wchar_t);
    RegQueryValueExW(hKey, L"InfPath", NULL, &type,
        (LPBYTE)outInfPath, &size);

    RegCloseKey(hKey);
}

/*
 * Comprueba si el dispositivo tiene un código de problema (error CM_PROB_*).
 * Devuelve 1 si hay problema y rellena problemCode.
 */
static int CheckDeviceProblem(
    HDEVINFO         devInfo,
    SP_DEVINFO_DATA* devData,
    DWORD* problemCode)
{
    (void)devInfo; /* no se usa directamente, CM trabaja con DevInst */

    /* Obtener estado del dispositivo via CM */
    ULONG status = 0, problem = 0;
    CONFIGRET cr = CM_Get_DevNode_Status(&status, &problem,
        devData->DevInst, 0);
    if (cr == CR_SUCCESS && (status & DN_HAS_PROBLEM)) {
        *problemCode = problem;
        return 1;
    }
    *problemCode = 0;
    return 0;
}

/*
 *  API pública exportada
 *   */

 /*
  * EnumerateAllDevices
  *
  * Enumera TODOS los dispositivos del sistema (presentes y no presentes).
  * Devuelve un puntero a DeviceList asignado en el heap.
  * El caller DEBE llamar a FreeDeviceList() cuando termine.
  *
  * Devuelve NULL en caso de error crítico.
  */
__declspec(dllexport)
DeviceList* __cdecl EnumerateAllDevices(void)
{
    /* Obtener handle al conjunto de todos los dispositivos */
    HDEVINFO devInfo = SetupDiGetClassDevsW(
        NULL,   /* todas las clases */
        NULL,   /* sin enumerador específico */
        NULL,   /* sin ventana padre */
        DIGCF_ALLCLASSES | DIGCF_PRESENT); /* solo presentes */

    if (devInfo == INVALID_HANDLE_VALUE) return NULL;

    /* Reservar lista inicial */
    DeviceList* list = (DeviceList*)HeapAlloc(
        GetProcessHeap(), HEAP_ZERO_MEMORY, sizeof(DeviceList));
    if (!list) {
        SetupDiDestroyDeviceInfoList(devInfo);
        return NULL;
    }

    list->capacity = 256;
    list->count = 0;
    list->records = (DeviceRecord*)HeapAlloc(
        GetProcessHeap(), HEAP_ZERO_MEMORY,
        list->capacity * sizeof(DeviceRecord));

    if (!list->records) {
        HeapFree(GetProcessHeap(), 0, list);
        SetupDiDestroyDeviceInfoList(devInfo);
        return NULL;
    }

    SP_DEVINFO_DATA devData = { 0 };
    devData.cbSize = sizeof(SP_DEVINFO_DATA);

    for (DWORD idx = 0;
        SetupDiEnumDeviceInfo(devInfo, idx, &devData);
        idx++)
    {
        /* Crecer el array si es necesario */
        if (list->count >= list->capacity) {
            int newCap = list->capacity * 2;
            DeviceRecord* newBuf = (DeviceRecord*)HeapReAlloc(
                GetProcessHeap(), HEAP_ZERO_MEMORY,
                list->records, newCap * sizeof(DeviceRecord));
            if (!newBuf) break; /* no podemos continuar sin memoria */
            list->records = newBuf;
            list->capacity = newCap;
        }

        DeviceRecord* rec = &list->records[list->count];
        memset(rec, 0, sizeof(DeviceRecord));

        /* ---- Instance ID ---------------------------------------- */
        SetupDiGetDeviceInstanceIdW(
            devInfo, &devData,
            rec->instanceId,
            sizeof(rec->instanceId) / sizeof(wchar_t),
            NULL);

        /* ---- Propiedades básicas --------------------------------- */
        ReadStringProperty(devInfo, &devData, SPDRP_DEVICEDESC,
            rec->deviceDescription,
            sizeof(rec->deviceDescription) / sizeof(wchar_t));

        ReadStringProperty(devInfo, &devData, SPDRP_MFG,
            rec->manufacturer,
            sizeof(rec->manufacturer) / sizeof(wchar_t));

        /* ---- Class GUID ----------------------------------------- */
        wchar_t guidBuf[64] = { 0 };
        ReadStringProperty(devInfo, &devData, SPDRP_CLASSGUID,
            guidBuf, 64);
        wcsncpy_s(rec->classGuid,
            sizeof(rec->classGuid) / sizeof(wchar_t),
            guidBuf, _TRUNCATE);

        /* ---- Hardware IDs --------------------------------------- */
        rec->hardwareIdCount = ReadHardwareIds(
            devInfo, &devData,
            rec->hardwareIds,
            sizeof(rec->hardwareIds) / sizeof(HardwareIdEntry));

        /* ---- Info del driver ------------------------------------ */
        ReadDriverVersionInfo(
            devInfo, &devData,
            rec->driverVersion, sizeof(rec->driverVersion) / sizeof(wchar_t),
            rec->driverDate, sizeof(rec->driverDate) / sizeof(wchar_t),
            rec->infPath, sizeof(rec->infPath) / sizeof(wchar_t));

        rec->hasDriver = (rec->driverVersion[0] != L'\0') ? 1 : 0;

        /* ---- Estado / problema ---------------------------------- */
        rec->isProblem = CheckDeviceProblem(
            devInfo, &devData, &rec->problemCode);

        list->count++;
    }

    SetupDiDestroyDeviceInfoList(devInfo);
    return list;
}

/*
 * FreeDeviceList
 *
 * Libera la memoria asignada por EnumerateAllDevices().
 * SIEMPRE llamar después de terminar de usar el DeviceList.
 */
__declspec(dllexport)
void __cdecl FreeDeviceList(DeviceList* list)
{
    if (!list) return;
    if (list->records) {
        HeapFree(GetProcessHeap(), 0, list->records);
        list->records = NULL;
    }
    HeapFree(GetProcessHeap(), 0, list);
}

/*
 * GetDeviceCount
 *
 * Devuelve el número de dispositivos en la lista.
 * Helper seguro para P/Invoke sin tener que leer campos de struct.
 */
__declspec(dllexport)
int __cdecl GetDeviceCount(DeviceList* list)
{
    if (!list) return 0;
    return list->count;
}

/*
 * GetDevice
 *
 * Devuelve un puntero al DeviceRecord en la posición index.
 * El puntero es válido mientras no se llame FreeDeviceList().
 * Devuelve NULL si index está fuera de rango.
 */
__declspec(dllexport)
DeviceRecord* __cdecl GetDevice(DeviceList* list, int index)
{
    if (!list || index < 0 || index >= list->count) return NULL;
    return &list->records[index];
}

/*
 *  DllMain — mínimo, solo para satisfacer el linker
 *   */
BOOL APIENTRY DllMain(HMODULE hModule, DWORD reason, LPVOID reserved)
{
    (void)hModule;
    (void)reserved;
    switch (reason) {
    case DLL_PROCESS_ATTACH:
    case DLL_THREAD_ATTACH:
    case DLL_THREAD_DETACH:
    case DLL_PROCESS_DETACH:
        break;
    }
    return TRUE;
}