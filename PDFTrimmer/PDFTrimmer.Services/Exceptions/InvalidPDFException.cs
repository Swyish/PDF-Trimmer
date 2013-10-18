using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDFTrimmer.Services
{
    public class InvalidPDFException : TrimmerException
    {
        public override string Message
        {
            get
            {
                return "Cannot read the PDF file. Please make sure the PDF file is not corrupted.";
            }
        }
    }
}
