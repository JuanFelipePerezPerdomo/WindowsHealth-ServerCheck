using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using WindowsHealth_ServerCheck.Helpers;
using WindowsHealth_ServerCheck.Models;

namespace WindowsHealth_ServerCheck.Reports
{
    public class ReportBuilder
    {
        private const string ColorPrimary = "#191970";
        private const string ColorDfServer = "#fb9404";
        private const string ColorTableHead = "#2E3D6E";
        private const string ColorBorder = "#BDC3C7";
        private const string ColorHeaderBg = "#D0D5E8";

        public static void Generate(AuditResult audit, bool includesUpdate = true)
        {
            using SaveFileDialog dialog = new();
            dialog.Title = "Guardar Informe";
            dialog.Filter = "PDF (*.pdf) | *.pdf";
            dialog.FileName = $"Informe_Servidor_{audit.Date:yyyy-MM-dd_HH-mm}";

            if (dialog.ShowDialog() != DialogResult.OK)
                return;

            string path = dialog.FileName;

            byte[] watermarkBytes;
            // Cargamos el logo para el fondo de página (watermark).
            // con transparencia incorporada (~15-20% de opacidad recomendada).
            using (MemoryStream ms = new MemoryStream())
            {
                // 1. Obtenemos el Bitmap del recurso
                Properties.Resources.copican_logo.Save(ms, System.Drawing.Imaging.ImageFormat.Png);

                // 2. Lo convertimos a byte[]
                byte[] copican_logo = ms.ToArray();

                // 3. llamamos a la funcion del helper que aplica transparencia
                watermarkBytes = FormatBytesHelper.TransparentImages(copican_logo, 0.06f);
            }

            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(40);
                    page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Arial"));

                    // Fondo de página (watermark) 
                    // Se dibuja DEBAJO de todo el contenido. La imagen debe
                    // tener transparencia para no tapar el texto.
                    page.Background()
                        .AlignCenter()
                        .AlignMiddle()
                        .Width(380)
                        .Image(watermarkBytes)
                        .FitWidth();

                    page.Header().Element(c => ComposeHeader(c, audit));

                    page.Content().PaddingTop(15).Column(col =>
                    {
                        col.Spacing(20);

                        if (audit.CleanupExecuted && audit.CleanUp != null)
                            ComposeCleanUpSection(col, audit.CleanUp);

                        if (audit.SmartExecuted && audit.Disks?.Count > 0)
                            ComposeSmartSection(col, audit.Disks);

                        if (audit.UpdatesExecuted && audit.Updates != null)
                            ComposeUpdatesSection(col, audit.Updates, includesUpdate);

                        if (audit.DriversExecuted && audit.Drivers != null)
                            ComposeDriversSection(col, audit.Drivers);

                        if (audit.DfServerExecuted && audit.DfServer != null)
                            ComposeDfServerSection(col, audit.DfServer);
                    });

                    page.Footer().AlignCenter().PaddingTop(8).Text(text =>
                    {
                        text.Span("Generado el ");
                        text.Span(audit.Date.ToString("dd/MM/yyyy HH:mm")).Bold();
                        text.Span("  |  Página ");
                        text.CurrentPageNumber();
                        text.Span(" de ");
                        text.TotalPages();
                    });
                });
            }).GeneratePdf(path);

            MessageBox.Show("Informe generado correctamente.", "Informe PDF",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // Cabecera
        private static void ComposeHeader(IContainer container, AuditResult audit)
        {
            container.BorderBottom(2)
                .BorderColor(ColorPrimary)
                .PaddingBottom(8)
                .PaddingTop(8)
                .Row(row =>
            {
                row.RelativeItem().Column(col =>
                {
                    col.Item().Text("Health Check — Informe de Servidor")
                        .FontSize(18).Bold().FontColor(ColorPrimary);

                    col.Item().Row(r =>
                    {
                        r.AutoItem().Text($"Equipo: {Environment.MachineName}").SemiBold()
                            .FontSize(10).FontColor("#7F8C8D");
                        r.AutoItem().Text("   |   ").FontSize(10).FontColor("#BDC3C7");
                        r.AutoItem().Text($"  Técnico: {audit.TechnicianName}").SemiBold()
                            .FontSize(10).FontColor("#7F8C8D");
                        r.AutoItem().Text("   |   ").FontSize(10).FontColor("#BDC3C7");
                        r.AutoItem().Text($"  Incluye DF-Server: {(audit.IsDfServerTechnician ? "Sí" : "No")}").SemiBold()
                            .FontSize(10).FontColor(audit.IsDfServerTechnician ? ColorDfServer : "#7F8C8D");
                    });
                });
            });
        }

        // Limpieza
        private static void ComposeCleanUpSection(ColumnDescriptor col, CleanUpResult cleanUp)
        {
            col.Item().Element(SectionTitle("Limpieza de Archivos Temporales"));
            col.Item().Table(table =>
            {
                table.ColumnsDefinition(c => { c.RelativeColumn(); c.RelativeColumn(); });
                table.Header(header =>
                {
                    header.Cell().Element(HeaderCell).Text("Concepto");
                    header.Cell().Element(HeaderCell).Text("Resultado");
                });
                AddRow(table, "Archivos eliminados", cleanUp.DeleteFiles.ToString());
                AddRow(table, "Carpetas eliminadas", cleanUp.DeleteDirs.ToString());
                AddRow(table, "Espacio liberado", FormatBytesHelper.FormatBytes(cleanUp.FreedBytes));
            });
        }

        // S.M.A.R.T 
        private static void ComposeSmartSection(ColumnDescriptor col, List<SmartResult> disks)
        {
            col.Item().Element(SectionTitle("Protocolo S.M.A.R.T"));

            col.Item().Table(table =>
            {
                table.ColumnsDefinition(c =>
                {
                    c.RelativeColumn(3);
                    c.RelativeColumn(2);
                    c.RelativeColumn(2);
                    c.RelativeColumn(2);
                    c.RelativeColumn(2);
                    c.RelativeColumn(2);
                });

                table.Header(header =>
                {
                    header.Cell().Element(HeaderCell).Text("Disco");
                    header.Cell().Element(HeaderCell).Text("Tipo");
                    header.Cell().Element(HeaderCell).Text("Falla inminente");
                    header.Cell().Element(HeaderCell).Text("Temperatura");
                    header.Cell().Element(HeaderCell).Text("Horas de uso");
                    header.Cell().Element(HeaderCell).Text("Salud del disco");
                });

                foreach (SmartResult disk in disks)
                {
                    table.Cell().Border(1).BorderColor(ColorBorder).Padding(5)
                        .Text(disk.DiskName).FontSize(9);
                    table.Cell().Border(1).BorderColor(ColorBorder).Padding(5)
                        .Text(disk.InterfaceType ?? "Unknown").FontSize(9);

                    // Falla inminente
                    if (!disk.HasSmartData)
                        table.Cell().Border(1).BorderColor(ColorBorder).Padding(5)
                            .Text(t => t.Span("N/A").FontColor("#95A5A6").FontSize(9));
                    else if (!disk.PredictFailureResolved)
                        table.Cell().Border(1).BorderColor(ColorBorder).Padding(5)
                            .Text(t => t.Span("N/D").FontColor("#95A5A6").FontSize(9));
                    else
                        table.Cell().Border(1).BorderColor(ColorBorder).Padding(5)
                            .Text(t => t.Span(disk.PredictFailure ? "SÍ" : "No")
                                .FontColor(disk.PredictFailure ? "#E74C3C" : "#27AE60")
                                .Bold().FontSize(9));

                    // Temperatura
                    if (!disk.HasSmartData || disk.Temperature == 0)
                        table.Cell().Border(1).BorderColor(ColorBorder).Padding(5)
                            .Text(t => t.Span("N/D").FontColor("#95A5A6").FontSize(9));
                    else
                        table.Cell().Border(1).BorderColor(ColorBorder).Padding(5)
                            .Text(t => t.Span($"{disk.Temperature} °C")
                                .FontColor(disk.Temperature < 45 ? "#27AE60"
                                    : disk.Temperature <= 55 ? "#E67E22" : "#E74C3C")
                                .FontSize(9));

                    // Horas de uso
                    if (!disk.HasSmartData || disk.HoursUsed == 0)
                        table.Cell().Border(1).BorderColor(ColorBorder).Padding(5)
                            .Text(t => t.Span("N/D").FontColor("#95A5A6").FontSize(9));
                    else
                        table.Cell().Border(1).BorderColor(ColorBorder).Padding(5)
                            .Text(t => t.Span($"{disk.HoursUsed} h")
                                .FontColor(disk.HoursUsed < 20000 ? "#27AE60"
                                    : disk.HoursUsed < 40000 ? "#E67E22" : "#E74C3C")
                                .FontSize(9));

                    // Salud
                    if (!disk.HasHealthData)
                        table.Cell().Border(1).BorderColor(ColorBorder).Padding(5)
                            .Text(t => t.Span("N/D").FontColor("#95A5A6").FontSize(9));
                    else
                        table.Cell().Border(1).BorderColor(ColorBorder).Padding(5)
                            .Text(t => t.Span($"{disk.HealthPercent}%")
                                .FontColor(disk.HealthPercent >= 90 ? "#27AE60"
                                    : disk.HealthPercent >= 60 ? "#E67E22" : "#E74C3C")
                                .FontSize(9));
                }
            });
        }

        // Windows Update
        private static void ComposeUpdatesSection(ColumnDescriptor col, UpdateResult updates, bool includesUpdate)
        {
            col.Item().Element(SectionTitle("Windows Update"));
            col.Item().Table(table =>
            {
                table.ColumnsDefinition(c => { c.RelativeColumn(); c.RelativeColumn(); });
                table.Header(header =>
                {
                    header.Cell().Element(HeaderCell).Text("Concepto");
                    header.Cell().Element(HeaderCell).Text("Resultado");
                });
                AddRow(table, "Modo de ejecución",
                    updates.IsQueryOnly ? "Consulta" : "Instalación");
                AddRow(table, "Actualizaciones encontradas", updates.UpdatesFound.ToString());

                if (!updates.IsQueryOnly)
                {
                    AddRow(table, "Actualizaciones instaladas", updates.UpdatesInstalled.ToString());
                    AddRow(table, "Estado",
                        updates.Success ? "Correcto" : "Con errores",
                        updates.Success ? "#27AE60" : "#E74C3C");
                }
            });

            // añadimos includesUpdate para que oculte esta seccion o no dependiendo de la respuesta del tecnico
            if (includesUpdate && updates.UpdateTitles?.Count > 0)
            {
                // La etiqueta cambia según si se instalaron o solo se detectaron
                string listTitle = updates.IsQueryOnly
                    ? "Actualizaciones disponibles (pendientes de instalar):"
                    : "Actualizaciones instaladas:";

                col.Item().PaddingTop(8).Text(listTitle).Bold();
                foreach (string title in updates.UpdateTitles)
                    col.Item().PaddingLeft(10).Text($"• {title}").FontSize(9);
            }
        }

        // Drivers
        private static void ComposeDriversSection(ColumnDescriptor col, DriversResult drivers)
        {
            col.Item().Element(SectionTitle("Escaneo de Drivers"));
            col.Item().Table(table =>
            {
                table.ColumnsDefinition(c => { c.RelativeColumn(); c.RelativeColumn(); });
                table.Header(header =>
                {
                    header.Cell().Element(HeaderCell).Text("Concepto");
                    header.Cell().Element(HeaderCell).Text("Resultado");
                });
                AddRow(table, "Drivers escaneados", drivers.TotalDrivers.ToString());
                AddRow(table, "Drivers desactualizados", drivers.OutdatedDrivers.ToString(),
                    drivers.OutdatedDrivers > 0 ? "#E74C3C" : "#27AE60");
            });

            var outdated = drivers.Drivers.FindAll(d => d.IsOutdated);
            if (outdated.Count > 0)
            {
                col.Item().PaddingTop(10).Text("Drivers con actualización disponible:").Bold();
                col.Item().Table(table =>
                {
                    table.ColumnsDefinition(c =>
                    {
                        c.RelativeColumn(3);
                        c.RelativeColumn(2);
                        c.RelativeColumn(1);
                    });
                    table.Header(header =>
                    {
                        header.Cell().Element(HeaderCell).Text("Dispositivo");
                        header.Cell().Element(HeaderCell).Text("Versión instalada");
                        header.Cell().Element(HeaderCell).Text("Fecha driver");
                    });
                    foreach (DriverInfo d in outdated)
                    {
                        table.Cell().Border(1).BorderColor(ColorBorder).Padding(5)
                            .Text(text => text.Span(d.DeviceName).FontColor("#E74C3C").Bold());
                        table.Cell().Border(1).BorderColor(ColorBorder).Padding(5).Text(d.DriverVersion);
                        table.Cell().Border(1).BorderColor(ColorBorder).Padding(5).Text(d.DriverDate);
                    }
                });
            }
        }

        // DF-Server
        private static void ComposeDfServerSection(ColumnDescriptor col, DfServerData df)
        {
            col.Item().Element(SectionTitleDf("Datos DF-Server"));

            col.Item().PaddingTop(6).Text("Digitalización certificada").Bold();
            col.Item().Table(table =>
            {
                table.ColumnsDefinition(c => { c.RelativeColumn(); c.RelativeColumn(); });
                table.Header(h =>
                {
                    h.Cell().Element(HeaderCellDf).Text("Concepto");
                    h.Cell().Element(HeaderCellDf).Text("Estado");
                });
                AddRow(table, "Tiene digitalización certificada",
                    df.HasCertifiedDigitization ? "Sí" : "No");
                if (df.HasCertifiedDigitization)
                    AddRow(table, "Configurada correctamente",
                        df.HasConfigureDigitization ? "Sí" : "No",
                        df.HasConfigureDigitization ? "#27AE60" : "#E74C3C");
            });

            col.Item().PaddingTop(10).Text("DF-Signature").Bold();
            col.Item().Table(table =>
            {
                table.ColumnsDefinition(c => { c.RelativeColumn(); c.RelativeColumn(); });
                table.Header(h =>
                {
                    h.Cell().Element(HeaderCellDf).Text("Concepto");
                    h.Cell().Element(HeaderCellDf).Text("Estado");
                });
                AddRow(table, "Tiene firmas DF-Signature", df.HasDfSignature ? "Sí" : "No");
                if (df.HasDfSignature)
                {
                    AddRow(table, "Número de firmas", df.DfSignatureCount.ToString());
                    if (df.DfSignatureCount <= 100)
                        AddRow(table, "Cliente notificado",
                            df.ClientNotificateSignature ? "Sí" : "No",
                            df.ClientNotificateSignature ? "#27AE60" : "#E74C3C");
                }
            });

            col.Item().PaddingTop(10).Text("Certificados digitales").Bold();
            col.Item().Table(table =>
            {
                table.ColumnsDefinition(c => { c.RelativeColumn(); c.RelativeColumn(); });
                table.Header(h =>
                {
                    h.Cell().Element(HeaderCellDf).Text("Concepto");
                    h.Cell().Element(HeaderCellDf).Text("Estado");
                });
                AddRow(table, "Tiene certificados digitales", df.HasCertificates ? "Sí" : "No");
                if (df.HasCertificates)
                    AddRow(table, "Número de certificados", df.Certificate.Count.ToString());
            });

            if (df.HasCertificates && df.Certificate.Count > 0)
            {
                col.Item().PaddingTop(8).Table(table =>
                {
                    table.ColumnsDefinition(c =>
                    {
                        c.RelativeColumn(3);
                        c.RelativeColumn(2);
                        c.RelativeColumn(2);
                        c.RelativeColumn(2);
                    });
                    table.Header(h =>
                    {
                        h.Cell().Element(HeaderCellDf).Text("Certificado");
                        h.Cell().Element(HeaderCellDf).Text("Caducidad");
                        h.Cell().Element(HeaderCellDf).Text("Estado");
                        h.Cell().Element(HeaderCellDf).Text("Cliente avisado");
                    });

                    foreach (CertificateInfo cert in df.Certificate)
                    {
                        string estadoColor = cert.Status switch
                        {
                            CertificateStatus.Expired => "#E74C3C",
                            CertificateStatus.ExpiringSoon => "#E67E22",
                            _ => "#27AE60",
                        };
                        string estadoText = cert.Status switch
                        {
                            CertificateStatus.Expired => "Caducado",
                            CertificateStatus.ExpiringSoon => "Caduca pronto",
                            _ => "Vigente",
                        };

                        table.Cell().Border(1).BorderColor(ColorBorder).Padding(5).Text(cert.Name);
                        table.Cell().Border(1).BorderColor(ColorBorder).Padding(5)
                            .Text(cert.ExpirationDate.ToString("dd/MM/yyyy"));
                        table.Cell().Border(1).BorderColor(ColorBorder).Padding(5)
                            .Text(t => t.Span(estadoText).FontColor(estadoColor).Bold());

                        if (cert.Status != CertificateStatus.Valid)
                            table.Cell().Border(1).BorderColor(ColorBorder).Padding(5)
                                .Text(t => t.Span(cert.ClientNotificade ? "Sí" : "No")
                                    .FontColor(cert.ClientNotificade ? "#27AE60" : "#E74C3C").Bold());
                        else
                            table.Cell().Border(1).BorderColor(ColorBorder).Padding(5).Text("—");
                    }
                });
            }
        }

        // Helpers de estilo

        private static Action<IContainer> SectionTitle(string title) => container =>
            container.Background(ColorPrimary).Padding(8).Text(title)
                .FontSize(12).Bold().FontColor("#FFFFFF");

        private static Action<IContainer> SectionTitleDf(string title) => container =>
            container.Background(ColorDfServer).Padding(8).Text(title)
                .FontSize(12).Bold().FontColor("#FFFFFF");

        private static IContainer HeaderCell(IContainer container) =>
            container.Background(ColorHeaderBg)
            .PaddingVertical(8)
             .PaddingHorizontal(5)
            .BorderColor("#95A5A6");

        private static IContainer HeaderCellDf(IContainer container) =>
            container.Background("#FDE8C0")
            .PaddingVertical(8)
            .PaddingHorizontal(5)
            .BorderColor("#E8A020");

        private static void AddRow(TableDescriptor table, string label, string value, string hexColor = null)
        {
            table.Cell().Border(1).BorderColor(ColorBorder).Padding(5).Text(label);
            if (hexColor != null)
                table.Cell().Border(1).BorderColor(ColorBorder).Padding(5)
                    .Text(text => text.Span(value).FontColor(hexColor).Bold());
            else
                table.Cell().Border(1).BorderColor(ColorBorder).Padding(5).Text(value);
        }
    }
}