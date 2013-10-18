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

                var page = pdfReader.GetPageSize(1);
                response.IsSuccessful = true;
            }

            return response;
        }

        public TrimmerResponse Trim(TrimmerRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
