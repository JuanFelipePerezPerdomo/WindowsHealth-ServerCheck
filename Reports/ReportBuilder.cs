using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using WindowsHealth_ServerCheck.Helpers;
using WindowsHealth_ServerCheck.Models;

namespace WindowsHealth_ServerCheck.Reports
{
    public class ReportBuilder
    {
        public static void Generate(AuditResult audit)
        {
            using (SaveFileDialog dialog = new SaveFileDialog())
            {
                dialog.Title = "Guardar Informe";
                dialog.Filter = "PDF (*.pdf) | *.pdf";
                dialog.FileName = $"Informe_Servidor_{audit.Date:yyyy-MM-dd_HH-mm}";

                if (dialog.ShowDialog() != DialogResult.OK)
                    return;

                string path = dialog.FileName;

                Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Size(PageSizes.A4);
                        page.Margin(40);
                        page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Arial"));

                        page.Header().Element(ComposeHeader);

                        page.Content().Column(col =>
                        {
                            col.Spacing(20);

                            if (audit.CleanupExecuted && audit.CleanUp != null)
                                ComposeCleanUpSection(col, audit.CleanUp);

                            if (audit.SmartExecuted && audit.Disks?.Count > 0)
                                ComposeSmartSection(col, audit.Disks);

                            if (audit.UpdatesExecuted && audit.Updates != null)
                                ComposeUpdatesSection(col, audit.Updates);

                            if (audit.DriversExecuted && audit.Drivers != null)
                                ComposeDriversSection(col, audit.Drivers);

                            if (audit.DfServerExecuted && audit.DfServer != null)
                                ComposeDfServerSection(col, audit.DfServer);
                        });

                        page.Footer().AlignCenter().Text(text =>
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
        }

        private static void ComposeHeader(IContainer container)
        {
            container.BorderBottom(2).BorderColor("#2C3E50").PaddingBottom(8).Row(row =>
            {
                row.RelativeItem().Column(col =>
                {
                    col.Item().Text("Health Check — Informe de Servidor")
                        .FontSize(18).Bold().FontColor("#2C3E50");
                    col.Item().Text($"Equipo: {Environment.MachineName}")
                        .FontSize(10).FontColor("#7F8C8D");
                });
            });
        }

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
                AddRow(table, "Fecha de ejecución", cleanUp.Date.ToString("dd/MM/yyyy HH:mm:ss"));
            });
        }

        private static void ComposeSmartSection(ColumnDescriptor col, List<SmartResult> disks)
        {
            col.Item().Element(SectionTitle("Protocolo S.M.A.R.T"));

            foreach (SmartResult disk in disks)
            {
                // Cabecera: nombre del disco + tipo de interfaz
                col.Item().PaddingBottom(2).Row(row =>
                {
                    row.RelativeItem().Text(disk.DiskName)
                        .FontSize(9).FontColor("#7F8C8D").Italic();
                    row.AutoItem().Text($"  [{disk.InterfaceType ?? "Unknown"}]")
                        .FontSize(9).FontColor("#95A5A6").Italic();
                });

                // Disco sin datos SMART (USB u otro bus externo)
                if (!disk.HasSmartData)
                {
                    col.Item().PaddingLeft(4).PaddingBottom(8)
                        .Text("SMART no disponible para este tipo de dispositivo.")
                        .FontSize(9).FontColor("#E67E22").Italic();
                    continue;
                }

                col.Item().Table(table =>
                {
                    table.ColumnsDefinition(c => { c.RelativeColumn(); c.RelativeColumn(); });
                    table.Header(header =>
                    {
                        header.Cell().Element(HeaderCell).Text("Atributo");
                        header.Cell().Element(HeaderCell).Text("Valor");
                    });

                    if (disk.Temperature > 0)
                        AddRow(table, "Temperatura", $"{disk.Temperature} °C",
                            disk.Temperature > 55 ? "#E74C3C" : null);

                    if (disk.HasHealthData)
                        AddRow(table, "Vida útil restante", $"{disk.HealthPercent}%",
                            disk.HealthPercent < 10 ? "#E74C3C" : null);

                    if (disk.HoursUsed > 0)
                        AddRow(table, "Horas de uso", $"{disk.HoursUsed} h");

                    if (disk.HasHealthData)
                        AddRow(table, "¿Falla inminente?",
                            disk.PredictFailure ? "SÍ" : "No",
                            disk.PredictFailure ? "#E74C3C" : "#27AE60");
                });

                col.Item().PaddingBottom(8);
            }
        }

        private static void ComposeUpdatesSection(ColumnDescriptor col, UpdateResult updates)
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
                AddRow(table, "Actualizaciones encontradas", updates.UpdatesFound.ToString());
                AddRow(table, "Actualizaciones instaladas", updates.UpdatesInstalled.ToString());
                AddRow(table, "Estado", updates.Success ? "Correcto" : "Con errores",
                    updates.Success ? "#27AE60" : "#E74C3C");
                AddRow(table, "Fecha de ejecución", updates.Date.ToString("dd/MM/yyyy HH:mm:ss"));
            });

            if (updates.UpdateTitles?.Count > 0)
            {
                col.Item().PaddingTop(8).Text("Actualizaciones instaladas:").Bold();
                foreach (string title in updates.UpdateTitles)
                    col.Item().PaddingLeft(10).Text($"• {title}").FontSize(9);
            }
        }

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
                AddRow(table, "Fecha de ejecución", drivers.Date.ToString("dd/MM/yyyy HH:mm:ss"));
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
                        table.Cell().Border(1).BorderColor("#BDC3C7").Padding(5)
                            .Text(text => text.Span(d.DeviceName).FontColor("#E74C3C").Bold());
                        table.Cell().Border(1).BorderColor("#BDC3C7").Padding(5).Text(d.DriverVersion);
                        table.Cell().Border(1).BorderColor("#BDC3C7").Padding(5).Text(d.DriverDate);
                    }
                });
            }
        }

        private static void ComposeDfServerSection(ColumnDescriptor col, DfServerData df)
        {
            col.Item().Element(SectionTitle("Datos DfServer"));

            // Digitalización Certificada
            col.Item().PaddingTop(6).Text("Digitalización certificada").Bold();
            col.Item().Table(table =>
            {
                table.ColumnsDefinition(c => { c.RelativeColumn(); c.RelativeColumn(); });
                table.Header(h => { h.Cell().Element(HeaderCell).Text("Concepto"); h.Cell().Element(HeaderCell).Text("Estado"); });
                AddRow(table, "Tiene digitalización certificada", df.HasCertifiedDigitization ? "Sí" : "No");
                if (df.HasCertifiedDigitization)
                    AddRow(table, "Configurada correctamente",
                        df.HasConfigureDigitization ? "Sí" : "No",
                        df.HasConfigureDigitization ? "#27AE60" : "#E74C3C");
            });

            // DfSignature
            col.Item().PaddingTop(10).Text("DfSignature").Bold();
            col.Item().Table(table =>
            {
                table.ColumnsDefinition(c => { c.RelativeColumn(); c.RelativeColumn(); });
                table.Header(h => { h.Cell().Element(HeaderCell).Text("Concepto"); h.Cell().Element(HeaderCell).Text("Estado"); });
                AddRow(table, "Tiene firmas DfSignature", df.HasDfSignature ? "Sí" : "No");
                if (df.HasDfSignature)
                {
                    AddRow(table, "Número de firmas", df.DfSignatureCount.ToString());
                    if (df.DfSignatureCount <= 100)
                        AddRow(table, "Cliente notificado",
                            df.ClientNotificateSignature ? "Sí" : "No",
                            df.ClientNotificateSignature ? "#27AE60" : "#E74C3C");
                }
            });

            // Certificados digitales
            col.Item().PaddingTop(10).Text("Certificados digitales").Bold();
            col.Item().Table(table =>
            {
                table.ColumnsDefinition(c => { c.RelativeColumn(); c.RelativeColumn(); });
                table.Header(h => { h.Cell().Element(HeaderCell).Text("Concepto"); h.Cell().Element(HeaderCell).Text("Estado"); });
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
                        h.Cell().Element(HeaderCell).Text("Certificado");
                        h.Cell().Element(HeaderCell).Text("Caducidad");
                        h.Cell().Element(HeaderCell).Text("Estado");
                        h.Cell().Element(HeaderCell).Text("Cliente avisado");
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

                        table.Cell().Border(1).BorderColor("#BDC3C7").Padding(5).Text(cert.Name);
                        table.Cell().Border(1).BorderColor("#BDC3C7").Padding(5)
                            .Text(cert.ExpirationDate.ToString("dd/MM/yyyy"));
                        table.Cell().Border(1).BorderColor("#BDC3C7").Padding(5)
                            .Text(t => t.Span(estadoText).FontColor(estadoColor).Bold());

                        // "Cliente avisado" solo aplica a caducados/próximos
                        if (cert.Status != CertificateStatus.Valid)
                            table.Cell().Border(1).BorderColor("#BDC3C7").Padding(5)
                                .Text(t => t.Span(cert.ClientNotificade ? "Sí" : "No")
                                    .FontColor(cert.ClientNotificade ? "#27AE60" : "#E74C3C").Bold());
                        else
                            table.Cell().Border(1).BorderColor("#BDC3C7").Padding(5).Text("—");
                    }
                });
            }
        }

        private static Action<IContainer> SectionTitle(string title) => container =>
            container.Background("#2C3E50").Padding(8).Text(title)
                .FontSize(12).Bold().FontColor("#FFFFFF");

        private static IContainer HeaderCell(IContainer container) =>
            container.Background("#BDC3C7").Padding(6).Border(1).BorderColor("#95A5A6");

        private static void AddRow(TableDescriptor table, string label, string value, string hexColor = null)
        {
            table.Cell().Border(1).BorderColor("#BDC3C7").Padding(5).Text(label);
            if (hexColor != null)
                table.Cell().Border(1).BorderColor("#BDC3C7").Padding(5)
                    .Text(text => text.Span(value).FontColor(hexColor).Bold());
            else
                table.Cell().Border(1).BorderColor("#BDC3C7").Padding(5).Text(value);
        }
    }
}