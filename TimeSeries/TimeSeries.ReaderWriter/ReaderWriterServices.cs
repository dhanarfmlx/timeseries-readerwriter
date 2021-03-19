using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeSeries.ReaderWriter
{
    public class ReaderWriterServices
    {
        public void Write(string file, string sensorData)
        {
            using (FileStream SourceStream = File.Open(file, FileMode.Append,FileAccess.Write,FileShare.Read))
            {
                byte[] result = new UTF8Encoding(true).GetBytes(sensorData+"\r\n");
                SourceStream.Write(result, 0, result.Length);
            }
        }

        public string Read(DateTime dt, int step, string FileContainer)
        {
            string date = dt.ToString("yyyy-MM-dd");
            string file = $@"{FileContainer}{date}.txt";
            byte[] result;

            if (File.Exists(file))
            {
                using (FileStream SourceStream = File.Open(file, FileMode.Open, FileAccess.Read,FileShare.Write))
                {
                    result = new byte[SourceStream.Length];
                    SourceStream.Read(result, 0, (int)SourceStream.Length);
                }
            }
            else
            {
                throw new Exception("File Not Found");
            }

            return Encoding.ASCII.GetString(result);
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

            string jsonFile = JsonConvert.SerializeObject(sd)+",";

            if (dt.Hour==23 && dt.Minute==59 && dt.Second==59)
            {
                jsonFile=jsonFile.TrimEnd(','); 
            }

            return jsonFile;
        }
    }
}
