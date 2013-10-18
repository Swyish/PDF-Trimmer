using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDFTrimmer.Services
{
    public class PDFTrimmerService : ITrimmerService
    {
        public DocInfoResponse GetDocInfo(DocInfoRequest request)
        {
            // Handling invalid requests
            if (request == null)
            {
                return new DocInfoResponse()
                {
                    IsSuccessful = false,
                    TrimmerException = new TrimmerRequestException()
                };
            }

            var response = new DocInfoResponse();

            PdfReader.unethicalreading = true;

            using (PdfReader pdfReader = new PdfReader(request.SourceFilePath))
            {
                using (var output = new MemoryStream())
                {
                    Document doc = new Document();
                    PdfSmartCopy smartCopy = new PdfSmartCopy(doc, output);

                    doc.Open();
                    PdfContentByte contentByte = smartCopy.DirectContent;

                    int totalPages = pdfReader.NumberOfPages;

                    // If the document has more than 4 pages
                    // Get 3 pages as a sample
                    if (totalPages > 3)
                    {
                        smartCopy.AddPage(smartCopy.GetImportedPage(pdfReader, ((int)totalPages / 3)));
                        smartCopy.AddPage(smartCopy.GetImportedPage(pdfReader, ((int)totalPages / 3) * 2));
                        smartCopy.AddPage(smartCopy.GetImportedPage(pdfReader, ((int)totalPages / 2) * 2));
                    }
                    else
                    {
                        // Taking only one if there is only one page
                        smartCopy.AddPage(smartCopy.GetImportedPage(pdfReader, 1));
                    }

                    doc.Close();
                    smartCopy.Close();

                    response.SamplePages = output.GetBuffer();
                }

                response.IsSuccessful = true;
            }

            return response;
        }

        public TrimmerResponse Trim(TrimmerRequest request)
        {
            PdfReader.unethicalreading = true;

            TrimmerResponse response = new TrimmerResponse();

            using (var pdfReader = new PdfReader(request.SourceFilePath))
            {
                PdfRectangle rect = new PdfRectangle(request.llx, request.lly,
                    PageSize.LETTER.Width - request.urx, PageSize.LETTER.Height - request.ury);

                using (var output = new MemoryStream())
                {
                    Document doc = new Document();
                    PdfSmartCopy smartCopy = new PdfSmartCopy(doc, output);


                    // Open the newly created document
                    doc.Open();
                    PdfContentByte contentByte = smartCopy.DirectContent;
                    // Loop through all pages of the source document
                    for (int i = 1; i <= pdfReader.NumberOfPages; i++)
                    {
                        // Get a page
                        var page = pdfReader.GetPageN(i);

                        // Apply the rectangle filter we created
                        page.Put(PdfName.CROPBOX, rect);
                        page.Put(PdfName.MEDIABOX, rect);

                        // Copy the content and insert into the new document
                        var copiedPage = smartCopy.GetImportedPage(pdfReader, i);
                        smartCopy.AddPage(copiedPage);
                    }

                    // Close the output document
                    doc.Close();
                    response.OutputFile = output.GetBuffer();
                }
            }

            if (response.OutputFile != null && response.OutputFile.Length > 0)
            {
                response.IsSuccessful = true;
            }
            else
            {
                response.IsSuccessful = false;
                response.TrimmerException = new TrimmerException();
            }

            return response;
        }
    }
}
