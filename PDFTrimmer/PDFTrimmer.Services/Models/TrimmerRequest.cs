using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDFTrimmer.Services
{
    public class TrimmerRequest : RequestBase
    {
        public int MarginLeft { get; set; }
        public int MarginBottom { get; set; }
        public int MarginRight { get; set; }
        public int MarginTop { get; set; }
    }
}
