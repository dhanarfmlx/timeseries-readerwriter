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
        public string runSimulation(DateTime startDate, DateTime finishDate, DateTime whenReadFile, DateTime whichDateToRead, string FilesContainer,int step)
        {
            DateTime clockRun = startDate; 
            string file = CreateFile(clockRun, FilesContainer);
            string readResult = "";
            bool exception = false;

            FileStream writerStream = createWriterStream(file);
            FileStream readerStream = null;

            while (clockRun.Day != finishDate.Day)
            {
                Write(writerStream, InputSensorData(clockRun));
                clockRun = clockRun.AddSeconds(1);

                if (clockRun.Hour == 0 && clockRun.Minute == 0 && clockRun.Second == 0)
                {
                    writerStream.Close();

                    file = CreateFile(clockRun, FilesContainer);
                    writerStream = createWriterStream(file);

                    deleteWeekly(FilesContainer);
                }

                if (clockRun.Equals(whenReadFile))
                {
                    if (clockRun.Day.Equals(whichDateToRead.Day))
                    {
                        writerStream.Flush();
                    }

                    try
                    {
                        readerStream = createReaderStream(whichDateToRead, FilesContainer);
                    }
                    catch (Exception ex)
                    {
                        exception = true;
                        readResult = ex.Message; 
                    }

                    if (exception == false)
                    {
                        readResult = Read(readerStream, step);
                    }
                }
            }

            if (exception == false)
            {
                readerStream.Close();
            }

            writerStream.Close();

            return readResult; 
        }

        public FileStream createWriterStream(string file)
        {
            return File.Open(file, FileMode.Append, FileAccess.Write, FileShare.Read); 
        }

        public FileStream createReaderStream(DateTime dt,string FilesContainer)
        {
            string date = dt.ToString("yyyy-MM-dd");
            string file = $@"{FilesContainer + date}.txt";

            if (!File.Exists(file))
            {
                throw new Exception("File Not Found");
            }
            else
            {
                return File.Open(file, FileMode.Open, FileAccess.Read, FileShare.Write);
            }
            
        }
        
        public void Write(FileStream SourceStream,string sensorData)
        {
            byte[] result = new UTF8Encoding(true).GetBytes(sensorData + "\r\n");
            SourceStream.Write(result, 0, result.Length);
        }

        public string Read(FileStream SourceStream, int step)
        {
            StringBuilder sB = new StringBuilder();

            using (StreamReader sr = new StreamReader(SourceStream)) 
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
                sd.sensorData[j] = Math.Round(random.NextDouble(),3);
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
