using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infor.HammPdfReading
{
    internal interface IContext
    {
        public string Text { get; }

        public List<ExtendedDetail> ExtendedDetails { get; }
        public List<Module> Modules { get; }
    }
}
