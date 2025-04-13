using System;
using System.Configuration;
using System.IO;
using GemBox.Pdf;
using GemBox.Pdf.Content;
using SalesMan.Models;
using Scriban;

namespace SalesMan.Services;

public class DefaultPrintService : IPrintService
{
    private readonly string printer = ConfigurationManager.AppSettings["printer"]!;

    private readonly string receiptDir = ConfigurationManager.AppSettings["receiptDir"]!;
    private readonly string receiptFontFamily = ConfigurationManager.AppSettings["receiptFontFamily"]!;
    private readonly string receiptTemplate = ConfigurationManager.AppSettings["receiptTemplate"]!;
    private const int receiptMargin = 5;

    private readonly IDialogService dialogService;

    public DefaultPrintService(IDialogService dialogService)
    {
        this.dialogService = dialogService;

        if (!Directory.Exists(receiptDir))
        {
            Directory.CreateDirectory(receiptDir);
        }

        // initialize gembox license
        ComponentInfo.SetLicense("FREE-LIMITED-KEY");
    }

    public bool Print(Order order)
    {
        var file = $"{receiptDir}/{order.Id}.pdf";
        try
        {
            ConvertToPdfDocument(order, file);
            PrintFile(file);

            return true;
        }
        catch (Exception ex)
        {
            dialogService.ShowMessage("Error", ex.ToString());
            return false;
        }
    }

    private void ConvertToPdfDocument(Order order, string file)
    {
        using (var doc = new PdfDocument())
        {
            // add page
            var page = doc.Pages.Add();

            using (var formattedText = new PdfFormattedText())
            {
                // format
                formattedText.FontFamily = new PdfFontFamily("./fonts", receiptFontFamily);

                // content
                var template = Template.Parse(File.ReadAllText(receiptTemplate), receiptTemplate);
                var content = template.Render(order);
                formattedText.Append(content);

                // draw
                page.Content.DrawText(formattedText, new PdfPoint(receiptMargin, receiptMargin));

                // resize
                page.SetMediaBox(formattedText.Width + receiptMargin * 2, formattedText.Height + receiptMargin * 2);
            }

            // write to file
            doc.Save(file);
        }
    }

    private void PrintFile(string file)
    {
        using (var doc = PdfDocument.Load(file))
        {
            if (string.IsNullOrWhiteSpace(printer))
            {
                doc.Print();
            }
            else
            {
                doc.Print(printer);
            }
        }
    }
}
