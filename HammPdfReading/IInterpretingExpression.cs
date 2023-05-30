using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infor.HammPdfReading
{
    internal interface IInterpretingExpression<T>
    {
        public void Interpet(Context<T> context);
    }
}
