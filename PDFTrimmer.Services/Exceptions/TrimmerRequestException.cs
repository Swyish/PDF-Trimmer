using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDFTrimmer.Services
{
    public class TrimmerRequestException : TrimmerException
    {
        public override string Message
        {
            get
            {
                return "Invalid request has been received";
            }
        }
    }
}
