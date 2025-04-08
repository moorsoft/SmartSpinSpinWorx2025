using SmartSpin.Hardware;
using SmartSpin.Support;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace SmartSpin.Model
{
    [Serializable]
    public class SampledProfile : ICloneable
    {
        public List<SampledPoints> AxisStorage = new List<SampledPoints>();

        /// <summary>
        /// Offsets for each axis 
        /// </summary>
        public double[] Offsets = new double[Globals.MAX_AXES];

        /// <summary>
        /// Playbackspeed (0-200%)
        /// </summary>
        public double PlaybackSpeed;

        /// <summary>
        /// SpindleSpeed (200-2000RPM)
        /// </summary>
        public double SpindleSpeed;

        /// <summary>
        /// Head Number 1 or 2 
        /// </summary>
        public int HeadNumber;

        /// <summary>
        /// Former Cycle for each memory 
        /// </summary>
        public int FormerCycle;

        // Start Positions of servo motors for each memory
        public double[] TargetPositions = new double[Globals.MAX_AXES];

        public SampledProfile()
        {
        }

        public SampledProfile(bool createStorage)
        {
            AxisStorage.Add(new SampledPoints());
            AxisStorage.Add(new SampledPoints());

            Clear();
        }

        [XmlIgnore]
        public int TotalSamples
        {
            get
            {
                return AxisStorage[0].Count;
            }
        }

        private bool PositionsEqual(int idx1, int idx2, bool Exact)
        {
            double Toler;

            for (int axisno = 0; axisno < 2; axisno++)
            {
                if (Exact) Toler = 0; else Toler = Machine.Axes[axisno].Tolerance;
                if (Math.Abs(AxisStorage[axisno][idx1] - AxisStorage[axisno][idx2]) > Toler) return false;
            }
            return true;
        }

        public void Add(SampledProfile Source, double[] Offsets, double[] Offsets2)
        {
            for (int i = 0; i < Source.AxisStorage[0].Count; i++)
            {
                AxisStorage[0].Add(Source.AxisStorage[0][i] + Offsets[0] + Offsets2[0]);
                AxisStorage[1].Add(Source.AxisStorage[1][i] + Offsets[1] + Offsets2[1]);
            }
        }

        public void Clear()
        {
            for (int i = 0; i < Globals.MAX_AXES; i++)
            {
                Offsets[i] = 0;
                TargetPositions[i] = 0;
            }
            PlaybackSpeed = 100;
            SpindleSpeed = Globals.BaseSpindleSpeed;
            FormerCycle = 0;
            AxisStorage[0].Clear();
            AxisStorage[1].Clear();
        }

        public void Assign(SampledProfile Source)
        {
            Offsets = Source.Offsets;
            PlaybackSpeed = Source.PlaybackSpeed;
            SpindleSpeed = Source.SpindleSpeed;
            HeadNumber = Source.HeadNumber;
            FormerCycle = Source.FormerCycle;
            TargetPositions = Source.TargetPositions;

            AxisStorage[0] = new SampledPoints(Source.AxisStorage[0]);
            AxisStorage[1] = new SampledPoints(Source.AxisStorage[1]);
        }

        public object Clone()
        {
            SampledProfile sp = (SampledProfile)this.MemberwiseClone();
            sp.AxisStorage = new List<SampledPoints> { 
            //sp.AxisStorage = new SampledPoints[2] {
                new SampledPoints(this.AxisStorage[0]),
                new SampledPoints(this.AxisStorage[1])
            };
            return sp;
        }

        public void TrimSamples(bool Exact, double ExtraTimeAdded = 0)
        {
            // first make sure both AxisStorage points are equal lengths
            if (AxisStorage[0].Count > AxisStorage[1].Count)
            {
                double d = AxisStorage[1][AxisStorage[1].Count - 1];
                while (AxisStorage[0].Count > AxisStorage[1].Count)
                {
                    AxisStorage[1].Add(d);
                }
            }
            if (AxisStorage[0].Count < AxisStorage[1].Count)
            {
                double d = AxisStorage[0][AxisStorage[0].Count - 1];
                while (AxisStorage[0].Count < AxisStorage[1].Count)
                {
                    AxisStorage[0].Add(d);
                }
            }

            // First remove dead positions from front of samples
            while (AxisStorage[0].Count > 5)
            {
                if (!(PositionsEqual(0, 1, Exact))) break;
                AxisStorage[0].RemoveAt(1);
                AxisStorage[1].RemoveAt(1);
            }

            // Now remove dead positions from end of samples
            while (AxisStorage[0].Count > 5)
            {
                if (!(PositionsEqual(AxisStorage[0].Count - 1, AxisStorage[0].Count - 2, Exact))) break;
                AxisStorage[0].RemoveAt(AxisStorage[0].Count - 2);
                AxisStorage[1].RemoveAt(AxisStorage[1].Count - 2);
            }

            double v0 = AxisStorage[0][AxisStorage[0].Count - 1];
            double v1 = AxisStorage[1][AxisStorage[0].Count - 1];
            // Now add some extra time at end to make sure the axis is settled
            for (int i = 0; i < (int)(ExtraTimeAdded / Globals.RecordSample); i++)
            {
                AxisStorage[0].Add(v0);
                AxisStorage[1].Add(v1);
            }
        }
    }
}
