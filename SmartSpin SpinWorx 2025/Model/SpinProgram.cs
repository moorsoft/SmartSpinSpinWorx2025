using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using System.IO.Compression;

namespace SmartSpin.Model
{
    [Serializable]
    public class SpinProgram
    {
        public const int MAX_MEMORIES_PLUS_1 = 11;

        public bool ProgramChanged;

        [XmlIgnore]
        public string ProgramFileName;

        public int MemoriesUsed;

        // Stores the actual spinning
        // index 0 stores the Overall settings or initial settings. 1-10 store the actual memories
        public SampledProfile[] MemS = new SampledProfile[MAX_MEMORIES_PLUS_1];

        public FormerProfile[] FormerCycles = new FormerProfile[10];

        // If yes then puts centre device up at end of cycle
        public bool UseCentreDevice;
        public bool UseBackStopDevice;

        public int SpindleDieSize;

        public int PartsCounter;

        public string Note;

        public SpinProgram()
        {
            for (int i = 0; i < MAX_MEMORIES_PLUS_1; i++) MemS[i] = new SampledProfile(true);
            for (int i = 0; i < 10; i++) FormerCycles[i] = new FormerProfile();
            NewProgram();
        }

        public void NewProgram()
        {
            foreach (var m in MemS) m.Clear();

            UseBackStopDevice = false;
            UseCentreDevice = false;

            SpindleDieSize = 1;

            MemoriesUsed = 0;
            PartsCounter = 0;
            ProgramFileName = string.Empty;
            Note = string.Empty;
            ProgramChanged = false;
        }

        //procedure ReadProgram(const FileName : String);
        //procedure WriteProgram(const FileName : String);

        public void DeleteLastMemory()
        {
            if (MemoriesUsed == 0) return;
            MemS[MemoriesUsed].Clear();
            MemoriesUsed--;
            ProgramChanged = true;
        }

        internal int CalculateSpinningMemories()
        {
            for (int i = MemoriesUsed; i > 1; i--)
            {
                if (MemS[i].TotalSamples > 10) return i;
            }
            return 0;
        }

        public static SpinProgram LoadProgram(string FileName)
        {
            using (Stream fstream = File.Open(FileName, FileMode.Open))
            using (GZipStream stream = new GZipStream(fstream, CompressionMode.Decompress))
            {
                //BinaryFormatter bFormatter = new BinaryFormatter();
                //SpinProgram program = (SpinProgram)bFormatter.Deserialize(stream);
                XmlSerializer serializer = new XmlSerializer(typeof(SpinProgram));
                SpinProgram program = (SpinProgram)serializer.Deserialize(stream);
                program.ProgramChanged = false;
                program.ProgramFileName = FileName;
                return program;
            }
        }

        public void SaveProgram(string FileName)
        {
            this.ProgramFileName = FileName;
            using (Stream fstream = File.Open(FileName, FileMode.Create))
            using (GZipStream stream = new GZipStream(fstream, CompressionMode.Compress))
            {
                //BinaryFormatter bFormatter = new BinaryFormatter();
                //bFormatter.Serialize(stream, this);
                XmlSerializer serializer = new XmlSerializer(typeof(SpinProgram));
                this.ProgramChanged = false;
                serializer.Serialize(stream, this);
            }
        }

        public void SaveProgramAsXml(string FileName)
        {
            this.ProgramFileName = FileName;
            using (Stream fstream = File.Open(FileName, FileMode.Create))
            {
                //BinaryFormatter bFormatter = new BinaryFormatter();
                //bFormatter.Serialize(stream, this);
                XmlSerializer serializer = new XmlSerializer(typeof(SpinProgram));
                this.ProgramChanged = false;
                serializer.Serialize(fstream, this);
            }
        }
    }
}
