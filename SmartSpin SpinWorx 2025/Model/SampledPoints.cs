using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSpin.Model
{
    [Serializable]
    public class SampledPoints : List<double>
    {
        public SampledPoints()
            : base()
        { }

        public SampledPoints(int capacity)
            : base(capacity)
        { }

        public SampledPoints(SampledPoints points)
            : base(points)
        { }

    }
}
