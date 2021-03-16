using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace TimeSeries.ReaderWriter
{
    public class ReaderWriterServices
    {
        public List<string> Write(string file, List<string> sensorDatas)
        {
            using (StreamWriter sw = new StreamWriter(file))
            {
                foreach (var sensorData in sensorDatas)
                {
                    sw.WriteLine(sensorData);
                }
            }
            sensorDatas.Clear();
            return sensorDatas;
        }

        public string Read(DateTime dt, int step, string FileContainer)
        {

            StringBuilder sB = new StringBuilder();
            string date = dt.ToString("yyyy-MM-dd");
            string file = $@"{FileContainer}{date}.txt";

            if (File.Exists(file))
            {
                if (new FileInfo(file).Length == 0)
                {
                    throw new Exception("File is stil written");
                }
                else
                {
                    using (StreamReader sr = new StreamReader(file))
                    {
                        int i = 0;
                        string line;
                        while ((line = sr.ReadLine()) != null)
                        {
                            if (i % step == 0)
                            {
                                sB.Append(line + Environment.NewLine);
                            }
                            i++;
                        }
                    }
                }
            }
            else
            {
                throw new Exception("File Not Found");
            }
            return sB.ToString();
        }

        public void deleteAllFiles(string FilesContainer)
        {
            foreach (string file in Directory.GetFiles($@"{FilesContainer}", "*.txt", SearchOption.AllDirectories))
            {
                File.Delete(file);
            }
        }

        public bool deleteWeekly(string FilesContainer)
        {
            //if there are 8 files ( 7 written files and 1 new file ) => delete 7 written files 
            List<string> listOfFiles = new List<string>();
            listOfFiles = Directory.GetFiles($@"{FilesContainer}", "*.txt", SearchOption.AllDirectories).ToList();

            if (listOfFiles.Count == 8)
            {
                while (listOfFiles.Count != 1)
                {
                    File.Delete(listOfFiles[0]);
                    listOfFiles.RemoveAt(0);
                }
                return true;
            }
            return false;
        }

        public int CountFiles(string FilesContainer)
        {
            return Directory.GetFiles($@"{FilesContainer}", "*.txt", SearchOption.AllDirectories).ToList().Count;
        }
        public string CreateFile(DateTime dt, string FilesContainer)
        {
            string date = dt.ToString("yyyy-MM-dd");
            string file = $@"{FilesContainer + date}.txt";

            if (!File.Exists(file))
            {
                using (File.Create(file)) { }
            }
            return file;
        }

        public string InputSensorData(DateTime dt)
        {
            SensorData sd = new SensorData();
            sd.datetime = dt.ToString("yyyy-MM-dd HH:mm:ss");

            Random random = new Random();
            sd.sensorData = new double[5];

            for (int j = 0; j < 5; j++)
            {
                sd.sensorData[j] = random.NextDouble();
            }

            string jsonFile = JsonConvert.SerializeObject(sd);
            return jsonFile;
        }
    }
}
