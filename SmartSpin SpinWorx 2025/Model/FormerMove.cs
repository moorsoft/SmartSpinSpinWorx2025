using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartSpin.Model
{
    [Serializable]
    public class FormerMove
    {
        public double HPosition { get; set; }

        public double HSpeed { get; set; }

        public double VPosition { get; set; }

        public double VSpeed { get; set; }

        public double Delay { get; set; }
    }
}
