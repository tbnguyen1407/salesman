using DragonAsia.SalesMan.Models;
using DragonAsia.SalesMan.Properties;
using PdfSharp.Drawing;
using PdfSharp.Drawing.Layout;
using PdfSharp.Pdf;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;

namespace DragonAsia.SalesMan.Helper
{
    class PrinterHelper
    {
        private static readonly string printApp = "native\\SumatraPDF.exe";
        private static readonly string printDir = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "/print/";

        static PrinterHelper()
        {
            if (!Directory.Exists(printDir))
                Directory.CreateDirectory(printDir);
        }

        public static bool Print(Order order)
        {
            try
            {
                ToPdf(order);

                if (File.Exists(printApp))
                {
                    string filePath = string.Format(@"""{0}/{1}.pdf""", printDir, order.Guid);
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = printApp,
                        Arguments = "-print-to-default " + filePath,
                        CreateNoWindow = true,
                        WindowStyle = ProcessWindowStyle.Hidden
                    });
                }

                return true;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return false;
            }
        }

        private static bool ToPdf(Order entity)
        {
            StringBuilder content = new StringBuilder();

            int lineSize = 42;
                        
            content.AppendLine("Dragon Asia");
            content.AppendLine("Spiegelstraße 3");
            content.AppendLine("68305 Mannheim-Luzenberg");
			content.AppendLine(Settings.Default.phone_number);
            content.AppendLine();
            content.AppendLine("Bestellung #" + entity.Guid);
            
            if (entity.TaxRefIncluded)
                content.AppendLine("Steuernummer " + Settings.Default.taxref_number);
            content.AppendLine();
            content.AppendLine(entity.Timestamp.ToString("MMM dd, yyyy HH:mm:ss"));
            content.AppendLine("------------------------------------");
            content.AppendLine(entity.Customer.Name);
            content.AppendLine(entity.Customer.Address);
            content.AppendLine(entity.Customer.Phone);
            content.AppendLine("------------------------------------");

            foreach (Item i in entity.Items.OrderBy(it => it.Code))
                content.AppendFormat("{0,-4} {1,-24}{2,6}€\n", i.Code, (i.Name.Length > 18 ? i.Name.Substring(0, 18) : i.Name) + " (x" + i.Quantity + ")", string.Format("{0:0.00}", i.Price * i.Quantity));

            content.AppendLine("------------------------------------");
            content.AppendLine("Gesamt (7% MwSt. enthalten)");
            content.AppendLine(string.Format("{0,35}€", string.Format("{0:0.00}", entity.TotalPrice)));
            content.AppendLine("------------------------------------");

            PdfDocument doc = new PdfDocument();
            doc.Info.Title = string.Format(@"Bestellung #{0}", entity.Guid);
            PdfPage page = doc.AddPage();
            page.Height = new XUnitPt((entity.Items.Count + 17) * lineSize);
            XGraphics gfx = XGraphics.FromPdfPage(page);
            XTextFormatter tf = new XTextFormatter(gfx);
            XFont font = new XFont("Consolas", 30);
            XBrush brush = XBrushes.Black;
            XRect rec = new XRect(5, 5, page.Width.Point, page.Height.Point);
            XStringFormat format = XStringFormats.TopLeft;

            tf.DrawString(content.ToString(), font, brush, rec, format);

            doc.Save(printDir + entity.Guid + ".pdf");
            return true;
        }

    }
}
