using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDFTrimmer.Services
{
    public abstract class ResponseBase
    {
        public byte[] OutputFile { get; set; }
        public bool IsSuccessful { get; set; }
        public TrimmerException TrimmerException { get; set; }
    }
}
