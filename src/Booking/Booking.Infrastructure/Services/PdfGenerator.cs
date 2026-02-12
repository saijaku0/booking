using Booking.Application.Appointments.Common.Interfaces;
using Booking.Application.Appointments.Dtos;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Booking.Infrastructure.Services
{

    public class PdfGenerator : IPdfGenerator
    {
        public PdfGenerator()
        {
            QuestPDF.Settings.License = LicenseType.Community;
        }

        public byte[] GenerateMedicalReport(MedicalReportDto data)
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    SetupPage(page);

                    page.Header().Element(header => ComposeHeader(header, "MEDICAL REPORT"));

                    page.Content().PaddingVertical(1, Unit.Centimetre).Column(col =>
                    {
                        ComposePatientDoctorInfo(col, data);

                        col.Item().PaddingVertical(10).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

                        col.Item().Element(c => ComposeSection(c, "Diagnosis", data.Diagnosis));

                        if (!string.IsNullOrWhiteSpace(data.TreatmentPlan))
                        {
                            col.Item().PaddingTop(10);
                            col.Item().Element(c => ComposeSection(c, "Treatment Plan & Prescriptions", data.TreatmentPlan));
                        }

                        if (!string.IsNullOrWhiteSpace(data.MedicalNotes))
                        {
                            col.Item().PaddingTop(10);
                            col.Item().Element(c => ComposeSection(c, "Medical Notes & Recommendations", data.MedicalNotes));
                        }
                    });

                    page.Footer().Element(ComposeFooter);
                });
            });

            return document.GeneratePdf();
        }

        public byte[] GeneratePrescription(MedicalReportDto data)
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    SetupPage(page);

                    page.Header().Element(header => ComposeHeader(header, "OFFICIAL PRESCRIPTION"));

                    page.Content().PaddingVertical(1, Unit.Centimetre).Column(col =>
                    {
                        ComposePatientDoctorInfo(col, data);
                        col.Item().PaddingVertical(10).LineHorizontal(2).LineColor(Colors.Black);
                        col.Item().PaddingTop(20);
                        col.Item().Text("Rx (Prescribed Medications):").FontSize(16).Bold();

                        col.Item().PaddingTop(10)
                           .Border(1)
                           .BorderColor(Colors.Grey.Lighten2)
                           .Padding(15)
                           .Text(data.TreatmentPlan ?? "No medications prescribed.")
                           .FontSize(14); 

                        col.Item().PaddingTop(50).Row(row =>
                        {
                            row.RelativeItem().Column(c =>
                            {
                                c.Item().Text("Doctor's Signature / Stamp:").FontSize(10).FontColor(Colors.Grey.Medium);
                                c.Item().PaddingTop(30).LineHorizontal(1);
                            });
                            row.ConstantItem(50); 
                            row.RelativeItem();
                        });
                    });

                    page.Footer().Element(ComposeFooter);
                });
            });

            return document.GeneratePdf();
        }

        private void SetupPage(PageDescriptor page)
        {
            page.Size(PageSizes.A4);
            page.Margin(2, Unit.Centimetre);
            page.PageColor(Colors.White);
            page.DefaultTextStyle(x => x.FontSize(12).FontFamily(Fonts.Arial));
        }

        private void ComposeHeader(IContainer container, string title)
        {
            container.Row(row =>
            {
                row.RelativeItem().Column(column =>
                {
                    column.Item().Text("Booking Medical Clinic").Bold().FontSize(18).FontColor(Colors.Blue.Darken2);
                    column.Item().Text("Forchheim, Bavaria, Germany").FontSize(10);
                    column.Item().Text($"Date: {DateTime.Now:d}").FontSize(10);
                });

                row.RelativeItem().AlignRight().AlignMiddle().Text(title).FontSize(20).Bold().FontColor(Colors.Grey.Lighten1);
            });
        }

        private void ComposePatientDoctorInfo(ColumnDescriptor column, MedicalReportDto data)
        {
            column.Item().Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.ConstantColumn(80); 
                    columns.RelativeColumn();   
                });

                table.Cell().Text("Doctor:").Bold().FontColor(Colors.Grey.Darken2);
                table.Cell().Text($"{data.DoctorName} ({data.DoctorSpecialty})");

                table.Cell().Text("Patient:").Bold().FontColor(Colors.Grey.Darken2);
                table.Cell().Text(data.PatientName).SemiBold();
            });
        }

        private void ComposeSection(IContainer container, string title, string content)
        {
            container.Column(col =>
            {
                col.Item().Background(Colors.Grey.Lighten4).Padding(5).Text(title).Bold().FontSize(12);

                col.Item().PaddingTop(5).PaddingLeft(5).Text(content);
            });
        }

        private void ComposeFooter(IContainer footer)
        {
            footer.AlignCenter().Text(x =>
            {
                x.Span("Generated automatically by Booking System | ").FontSize(9).FontColor(Colors.Grey.Medium);
                x.CurrentPageNumber().FontSize(9).FontColor(Colors.Grey.Medium);
            });
        }
    }
}