using iText.IO.Font;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using Microsoft.EntityFrameworkCore;
using WebApplication.Data;
using WebApplication.Models;

namespace WebApplication.Services
{
    public class CertificateService
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public CertificateService(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<byte[]> GenerateCertificateAsync(int certificateId)
        {
            var certificate = await _context.Certificates
                .Include(c => c.User)
                .Include(c => c.Exam)
                .FirstOrDefaultAsync(c => c.Id == certificateId);

            if (certificate == null)
                throw new ArgumentException("Сертификат не найден");

            using var memoryStream = new MemoryStream();
            using var writer = new PdfWriter(memoryStream);
            using var pdf = new PdfDocument(writer);
            using var document = new Document(pdf);

            var fontPath = Path.Combine(_env.WebRootPath, "fonts", "FreeSans.ttf");

            var font = PdfFontFactory.CreateFont(fontPath, PdfEncodings.IDENTITY_H);

            document.SetMargins(50, 50, 50, 50);
            var pageSize = pdf.GetDefaultPageSize();
            var canvas = new PdfCanvas(pdf.AddNewPage());
            canvas.SetStrokeColor(DeviceRgb.BLUE);
            canvas.SetLineWidth(2);
            canvas.Rectangle(40, 40, pageSize.GetWidth() - 80, pageSize.GetHeight() - 80);
            canvas.Stroke();

            document.Add(new Paragraph("★")
                .SetFont(font).SetFontSize(40)
                .SetTextAlignment(TextAlignment.CENTER)
                .SetFontColor(DeviceRgb.BLUE));

            document.Add(new Paragraph("MathProb")
                .SetFont(font).SetFontSize(16)
                .SetTextAlignment(TextAlignment.CENTER)
                .SetFontColor(new DeviceRgb(90, 90, 90)));

            document.Add(new Paragraph("СЕРТИФИКАТ")
                .SetFont(font).SetFontSize(28)
                .SetTextAlignment(TextAlignment.CENTER)
                .SetFontColor(DeviceRgb.BLUE)
                .SetMarginTop(20));

            document.Add(new Paragraph("о прохождении курса")
                .SetFont(font).SetFontSize(18)
                .SetTextAlignment(TextAlignment.CENTER)
                .SetFontColor(new DeviceRgb(100, 100, 100))
                .SetMarginTop(10));

            document.Add(new Paragraph(new string(' ', 50))
                .SetFont(font).SetFontSize(20)
                .SetTextAlignment(TextAlignment.CENTER)
                .SetFontColor(DeviceRgb.BLUE)
                .SetMarginTop(20));

            document.Add(new Paragraph(certificate.User.Name)
                .SetFont(font).SetFontSize(24)
                .SetTextAlignment(TextAlignment.CENTER)
                .SetFontColor(DeviceRgb.BLACK)
                .SetMarginTop(30));

            document.Add(new Paragraph("успешно прошел(а) курс по теории вероятностей и сдал(а) экзамен с результатом")
                .SetFont(font).SetFontSize(16)
                .SetTextAlignment(TextAlignment.CENTER)
                .SetFontColor(new DeviceRgb(80, 80, 80))
                .SetMarginTop(20));

            document.Add(new Paragraph($"{certificate.Exam.Score} баллов")
                .SetFont(font).SetFontSize(20)
                .SetTextAlignment(TextAlignment.CENTER)
                .SetFontColor(DeviceRgb.BLUE)
                .SetMarginTop(10));

            document.Add(new Paragraph($"Дата выдачи: {certificate.IssueDate:dd.MM.yyyy}")
                .SetFont(font).SetFontSize(14)
                .SetTextAlignment(TextAlignment.CENTER)
                .SetFontColor(new DeviceRgb(100, 100, 100))
                .SetMarginTop(40));

            document.Add(new Paragraph($"Код сертификата: {certificate.CertificateCode}")
                .SetFont(font).SetFontSize(12)
                .SetTextAlignment(TextAlignment.CENTER)
                .SetFontColor(new DeviceRgb(120, 120, 120))
                .SetMarginTop(10));

            document.Close();
            return memoryStream.ToArray();
        }
    }
}


