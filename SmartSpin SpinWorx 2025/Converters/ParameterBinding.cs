using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace SmartSpin.Converters
{
    public class ParameterBinding : Binding
    {
        public ParameterBinding(string path) : base(path)
        {
            this.StringFormat = @"{0:f2}";
        }
    }
}
