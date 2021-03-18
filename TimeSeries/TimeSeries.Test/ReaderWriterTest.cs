using NUnit.Framework;
using System;
using System.Collections.Generic;
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
                  Expected : no exception, write continue, read partial data in file X. 
        > Test2 : read unavailable file 
                  Expected : raise Exception 
        > Test3 : in the middle of writing data to file X=> read file Y
                  Expected : read all data in file Y
        > Test4 : create 9 files. delete automatically 7 first files when create 8th file. 
                  Expected : 2 files left 
         */


        [Test]
        [TestCase(5)]
        public async Task Test1(int step)
        {
            DateTime clockRun = new DateTime(2021, 3, 15, 23, 59, 0);
            string file = rw.CreateFile(clockRun, FilesContainer);
            string result=""; 

            while (clockRun.Day!=17)
            {
                rw.Write(file, rw.InputSensorData(clockRun));
                clockRun=clockRun.AddSeconds(1);
                
                if (clockRun.Hour == 0 && clockRun.Minute == 0 && clockRun.Second == 0)
                {
                    file = rw.CreateFile(clockRun, FilesContainer);
                }

                if (clockRun.Equals(new DateTime(2021,3,16,12,0,0)))
                {
                    result = await rw.ReadAsync(new DateTime(2021, 3, 16), step, FilesContainer);
                }
            }
            Assert.AreEqual(result.Length < 6500000, true);
            Assert.AreEqual(rw.CountFiles(FilesContainer), 3);
            Assert.AreEqual(rw.deleteWeekly(FilesContainer), false);
        }

        [Test]
        public void Test2()
        {
            var exception = Assert.ThrowsAsync<System.Exception>(async() => await rw.ReadAsync(new DateTime(2022, 3, 17), 5, FilesContainer));
            Assert.AreEqual("File Not Found", exception.Message);
        }

        [Test]
        [TestCase(5)]
        public async Task Test3(int step)
        {
            DateTime clockRun = new DateTime(2021, 3, 15, 0, 0, 0);
            string file = rw.CreateFile(clockRun, FilesContainer);
            string result = "";

            while (true)
            {
                rw.Write(file, rw.InputSensorData(clockRun));
                clockRun = clockRun.AddSeconds(1);

                if (clockRun.Hour == 0 && clockRun.Minute == 0 && clockRun.Second == 0)
                {
                    file = rw.CreateFile(clockRun, FilesContainer);
                }

                if (clockRun.Equals(new DateTime(2021, 3, 16, 1, 0, 0)))
                {
                    result = await rw.ReadAsync(new DateTime(2021, 3, 15), step, FilesContainer);
                    break;
                }
            }
            Assert.AreEqual(result.Length > 12817000, true);
            Assert.AreEqual(rw.CountFiles(FilesContainer), 2);
            Assert.AreEqual(rw.deleteWeekly(FilesContainer), false);
        }

        [Test]
        public void Test4()
        {
            DateTime clockRun = new DateTime(2021, 3, 8, 0, 0, 0);
            rw.CreateFile(clockRun, FilesContainer);

            while (clockRun.Day != 16)
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