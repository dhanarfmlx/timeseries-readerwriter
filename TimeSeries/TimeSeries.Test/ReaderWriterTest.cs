using NUnit.Framework;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TimeSeries.ReaderWriter;
namespace TimeSeries.Test
{
    public class Tests
    {
        ReaderWriterServices rw = new ReaderWriterServices();
        public string FilesContainer = @$"{Path.GetDirectoryName(Path.GetDirectoryName(TestContext.CurrentContext.TestDirectory))}\"; 

        [SetUp]
        public void Setup()
        {
            if (Directory.GetFiles(@$"{FilesContainer}", "*.txt", SearchOption.AllDirectories).ToList().Count != 0)
            {
                rw.deleteAllFiles(FilesContainer);
            }
        }

        /*
        [TEST CASES]
        > Test1 : in the middle of writing data to file X=> read file X
                  Expected : read partial data in file X, write file X completely
        > Test2 : read unavailable file 
                  Expected : raise Exception 
        > Test3 : in the middle of writing data to file X=> read file Y
                  Expected : read all data in file Y
        > Test4 : create 9 files. delete automatically 7 first files when create 8th file. 
                  Expected : 2 files left 
         */

        [Test]
        [TestCase(1)]
        public void Test1(int step)
        {
            DateTime clockRun = new DateTime(2021, 3, 15, 0, 0, 0);
            string file = rw.CreateFile(clockRun, FilesContainer);
            string readResult = "";

            FileStream writerStream = rw.createWriterStream(file);
            FileStream readerStream = null;

            while (clockRun.Day!=16)
            {
                rw.Write(writerStream, rw.InputSensorData(clockRun));
                clockRun = clockRun.AddSeconds(1);

                if (clockRun.Equals(new DateTime(2021, 3, 15, 12, 0, 0)))
                {
                    writerStream.Close(); // if not closed, the read data will not complete in last line

                    readerStream = rw.createReaderStream(new DateTime(2021, 3, 15), FilesContainer);
                    readResult = rw.Read(readerStream,step);

                    writerStream = rw.createWriterStream(file);
                }
            }
            readerStream.Close();
            writerStream.Close();

            Assert.AreEqual(readResult.Length < 4000000, true);
            Assert.AreEqual(rw.CountFiles(FilesContainer), 1);
            Assert.AreEqual(rw.deleteWeekly(FilesContainer), false);
        }
        
        [Test]
        public void Test2()
        {
            try
            {
                using (rw.createReaderStream(new DateTime(2022, 3, 16), FilesContainer));
            }
            catch(Exception ex)
            {
                Assert.AreEqual("File Not Found", ex.Message);
            }
        }

        [Test]
        [TestCase(1)]
        public void Test3(int step)
        {
            DateTime clockRun = new DateTime(2021, 3, 15, 0, 0, 0);
            string file = rw.CreateFile(clockRun, FilesContainer);
            string readResult = "";

            FileStream writerStream = rw.createWriterStream(file);
            FileStream readerStream = null;

            while (clockRun.Day != 19)
            {
                rw.Write(writerStream, rw.InputSensorData(clockRun));
                clockRun = clockRun.AddSeconds(1);

                if (clockRun.Hour == 0 && clockRun.Minute == 0 && clockRun.Second == 0)
                {
                    writerStream.Close();
                    file = rw.CreateFile(clockRun, FilesContainer);
                    writerStream = rw.createWriterStream(file);
                }

                if (clockRun.Equals(new DateTime(2021, 3, 18, 12, 0, 0)))
                {
                    writerStream.Close();

                    readerStream = rw.createReaderStream(new DateTime(2021, 3, 15), FilesContainer);
                    readResult = rw.Read(readerStream,step);

                    writerStream = rw.createWriterStream(file);
                }
            }
            readerStream.Close();
            writerStream.Close();

            Assert.AreEqual(readResult.Length > 7000000, true);
            Assert.AreEqual(rw.CountFiles(FilesContainer), 5);
            Assert.AreEqual(rw.deleteWeekly(FilesContainer), false);
        }

        [Test]
        public void Test4()
        {
            DateTime clockRun = new DateTime(2021, 4, 18, 0, 0, 0);
            rw.CreateFile(clockRun, FilesContainer);

            while (clockRun.Day != 26)
            {
                clockRun = clockRun.AddSeconds(1);

                if (clockRun.Hour == 0 && clockRun.Minute == 0 && clockRun.Second == 0)
                {
                    rw.CreateFile(clockRun, FilesContainer);
                    rw.deleteWeekly(FilesContainer);
                }
            }
            Assert.AreEqual(rw.CountFiles(FilesContainer), 2);
        }
    }
}