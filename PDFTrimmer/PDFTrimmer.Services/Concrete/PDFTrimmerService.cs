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
        /// <summary>
        /// Prepare the document for trimming
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public PrepareResponse Prepare(PrepareRequest request)
        {
            // Make sure the request is valid
            if (request == null)
            {
                return new PrepareResponse()
                {
                    IsSuccessful = false,
                    TrimmerException = new TrimmerRequestException()
                };
            }

            var response = new PrepareResponse();

            // Force the pdf read
            PdfReader.unethicalreading = true;

            // Read the uploaded document
            using (PdfReader pdfReader = new PdfReader(request.SourceFile))
            {
                int maxWidth = 0;
                int maxHeight = 0;

                response.PageCount = pdfReader.NumberOfPages;

                // Goes through all pages and look for the largest width and larget height of the document
                for (int i = 1; i <= pdfReader.NumberOfPages; i++)
                {
                    var pageSize = pdfReader.GetPageSize(i);
                    if (maxWidth < pageSize.Width)
                    {
                        maxWidth = (int)pageSize.Width;
                    }

                    if (maxHeight < pageSize.Height)
                    {
                        maxHeight = (int)pageSize.Height;
                    }
                }

                using (var output = new MemoryStream())
                {
                    Document doc = new Document(new Rectangle(maxWidth, maxHeight, 0));
                    PdfSmartCopy smartCopy = new PdfSmartCopy(doc, output);

                    doc.Open();
                    PdfContentByte contentByte = smartCopy.DirectContent;
                    PdfRectangle rect = new PdfRectangle(doc.PageSize);

                    // Loop through all pages of the source document
                    for (int i = 1; i <= pdfReader.NumberOfPages; i++)
                    {
                        // Get a page
                        var page = pdfReader.GetPageN(i);

                        page.Put(PdfName.CROPBOX, rect);
                        page.Put(PdfName.MEDIABOX, rect);

                        // Copy the content and insert into the new document
                        var copiedPage = smartCopy.GetImportedPage(pdfReader, i);
                        smartCopy.AddPage(copiedPage);
                    }

                    // Close the output document
                    doc.Close();
                    response.PreparedDoc = output.GetBuffer();
                }
            }
            response.IsSuccessful = true;

            return response;
        }

        /// <summary>
        /// Trim the document
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public TrimmerResponse Trim(TrimmerRequest request)
        {
            PdfReader.unethicalreading = true;

            TrimmerResponse response = new TrimmerResponse();

            using (var pdfReader = new PdfReader(request.SourceFile))
            {
                PdfRectangle rect = new PdfRectangle(request.MarginLeft, request.MarginBottom,
                    pdfReader.GetPageSizeWithRotation(1).Width - request.MarginRight, pdfReader.GetPageSizeWithRotation(1).Height - request.MarginTop);

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
