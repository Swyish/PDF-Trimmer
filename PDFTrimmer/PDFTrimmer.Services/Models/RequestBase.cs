using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDFTrimmer.Services
{
    public abstract class RequestBase
    {
        public string BaseFilePath { get; set; }
        public string SourceFileName { get; set; }
    }
}
