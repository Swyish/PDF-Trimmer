using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDFTrimmer.Services
{
    public class TrimmerRequest : RequestBase
    {
        public int llx { get; set; }
        public int lly { get; set; }
        public int urx { get; set; }
        public int ury { get; set; }
    }
}
