using SmartSpin.Support;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace SmartSpin.Model
{
    [Serializable]
    public class FormerProfile
    {
        [XmlIgnore]
        public int Idx { get; set; }

        /// <summary>
        /// SpindleSpeed (200-2000RPM)
        /// </summary>
        public double SpindleSpeed;

        public List<FormerMove> FormerMoves = new List<FormerMove>();

        public FormerProfile()
        {
            SpindleSpeed = Globals.BaseSpindleSpeed;
        }

        public void ClearAll()
        {
            FormerMoves = new List<FormerMove>();
        }

        public void AddPoint(FormerMove point)
        {
            FormerMoves.Add(point);
        }

        public void DeleteLast()
        {
            if (FormerMoves.Count > 0)
            {
                FormerMoves.Remove(FormerMoves[FormerMoves.Count - 1]);
            }
        }
    }
}
