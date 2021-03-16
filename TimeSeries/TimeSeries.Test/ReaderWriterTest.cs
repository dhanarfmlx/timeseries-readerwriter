using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TimeSeries.ReaderWriter;
namespace TimeSeries.Test
{
    public class Tests
    {
        ReaderWriterServices rw = new ReaderWriterServices();
        public string FilesContainer = @"C:\Users\DELL\yusuf-frmltrx\timeseries-readerwriter\TimeSeries\TimeSeries.ReaderWriter\Data\";

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
        > Test1 : want to write 1 day. in the middle of writing 1st day=> read 1st day
        > Test2 : want to write 4 days. in the middle of writing 2nd day=> read 3rd day
        > Test3 : want to write 3 days. in the middle of writing 3rd day=> read 2nd day 
        > Test4 : want to write 10 days. at the 8th day=>delete first 7 days. in the middle of writing 9th day=> read 8th day   
        > Test5 : want to write 10 days. at the 8th day=>delete first 7 days. in the middle of writing 8th day=> read 2nd day 
         */


        [Test]
        [TestCase(1)]
        [TestCase(5)]
        [TestCase(10)]
        public void Test1(int step)
        {
            int days = 1;
            DateTime clockRun = new DateTime(2021, 3, 15, 0, 0, 0);
            string file = rw.CreateFile(clockRun, FilesContainer);
            List<string> sensorDatas = new List<string>();

            int i = 0;
            while (i < (days * 86400))
            {
                clockRun = clockRun.AddSeconds(1);
                sensorDatas.Add(rw.InputSensorData(clockRun));

                if (clockRun.Hour == 23 && clockRun.Minute == 59 && clockRun.Second == 59)
                {
                    sensorDatas = rw.Write(file, sensorDatas);

                    file = rw.CreateFile(clockRun.AddSeconds(1), FilesContainer);

                    rw.deleteWeekly(FilesContainer);
                }

                if (i == 30000)
                {
                    var exception = Assert.Throws<System.Exception>(() => rw.Read(new DateTime(2021, 3, 15), step, FilesContainer));
                    Assert.AreEqual("File is stil written", exception.Message);
                    break;
                }

                i++;
            }

            Assert.AreEqual(rw.CountFiles(FilesContainer), 1);
            Assert.AreEqual(rw.deleteWeekly(FilesContainer), false);
        }

        [Test]
        [TestCase(1)]
        [TestCase(5)]
        [TestCase(10)]
        public void Test2(int step)
        {
            int days = 4;
            DateTime clockRun = new DateTime(2021, 3, 15, 0, 0, 0);
            string file = rw.CreateFile(clockRun, FilesContainer);
            List<string> sensorDatas = new List<string>();

            int i = 0;
            while (i < (days * 86400))
            {
                clockRun = clockRun.AddSeconds(1);
                sensorDatas.Add(rw.InputSensorData(clockRun));

                if (clockRun.Hour == 23 && clockRun.Minute == 59 && clockRun.Second == 59)
                {
                    sensorDatas = rw.Write(file, sensorDatas);

                    file = rw.CreateFile(clockRun.AddSeconds(1), FilesContainer);

                    rw.deleteWeekly(FilesContainer);
                }

                if (i == 90000)
                {
                    var exception = Assert.Throws<System.Exception>(() => rw.Read(new DateTime(2021, 3, 17), step, FilesContainer));
                    Assert.AreEqual("File Not Found", exception.Message);
                    break;
                }

                i++;
            }
            Assert.AreEqual(rw.CountFiles(FilesContainer), 2);
            Assert.AreEqual(rw.deleteWeekly(FilesContainer), false);
        }

        [Test]
        [TestCase(1)]
        [TestCase(5)]
        [TestCase(10)]
        public void Test3(int step)
        {
            int days = 3;
            DateTime clockRun = new DateTime(2021, 3, 15, 0, 0, 0);
            string file = rw.CreateFile(clockRun, FilesContainer);
            List<string> sensorDatas = new List<string>();

            int i = 0;
            while (i < (days * 86400))
            {
                clockRun = clockRun.AddSeconds(1);
                sensorDatas.Add(rw.InputSensorData(clockRun));

                if (clockRun.Hour == 23 && clockRun.Minute == 59 && clockRun.Second == 59)
                {
                    sensorDatas = rw.Write(file, sensorDatas);

                    file = rw.CreateFile(clockRun.AddSeconds(1), FilesContainer);

                    rw.deleteWeekly(FilesContainer);
                }

                if (i == (2 * 86400 + 1000))
                {
                    string result = rw.Read(new DateTime(2021, 3, 16), step, FilesContainer);
                    Assert.AreEqual(result.Length != 0, true);
                    break;
                }

                i++;
            }
            Assert.AreEqual(rw.CountFiles(FilesContainer), 3);
            Assert.AreEqual(rw.deleteWeekly(FilesContainer), false);
        }

        [Test]
        [TestCase(1)]
        [TestCase(5)]
        [TestCase(10)]
        public void Test4(int step)
        {
            int days = 10;
            DateTime clockRun = new DateTime(2021, 3, 15, 0, 0, 0);
            string file = rw.CreateFile(clockRun, FilesContainer);
            List<string> sensorDatas = new List<string>();

            int i = 0;
            while (i < (days * 86400))
            {
                clockRun = clockRun.AddSeconds(1);
                sensorDatas.Add(rw.InputSensorData(clockRun));

                if (clockRun.Hour == 23 && clockRun.Minute == 59 && clockRun.Second == 59)
                {
                    sensorDatas = rw.Write(file, sensorDatas);

                    file = rw.CreateFile(clockRun.AddSeconds(1), FilesContainer);

                    rw.deleteWeekly(FilesContainer);
                }

                if (i == (8 * 86400 + 1000))
                {
                    string result = rw.Read(new DateTime(2021, 3, 22), step, FilesContainer);
                    Assert.AreEqual(result.Length != 0, true);
                    break;
                }

                i++;
            }
            Assert.AreEqual(rw.CountFiles(FilesContainer), 2);
        }

        [Test]
        [TestCase(1)]
        [TestCase(5)]
        [TestCase(10)]
        public void Test5(int step)
        {
            int days = 10;
            DateTime clockRun = new DateTime(2021, 3, 15, 0, 0, 0);
            string file = rw.CreateFile(clockRun, FilesContainer);
            List<string> sensorDatas = new List<string>();

            int i = 0;
            while (i < (days * 86400))
            {
                clockRun = clockRun.AddSeconds(1);
                sensorDatas.Add(rw.InputSensorData(clockRun));

                if (clockRun.Hour == 23 && clockRun.Minute == 59 && clockRun.Second == 59)
                {
                    sensorDatas = rw.Write(file, sensorDatas);

                    file = rw.CreateFile(clockRun.AddSeconds(1), FilesContainer);

                    rw.deleteWeekly(FilesContainer);
                }

                if (i == (7 * 86400 + 1000))
                {
                    var exception = Assert.Throws<System.Exception>(() => rw.Read(new DateTime(2021, 3, 16), step, FilesContainer));
                    Assert.AreEqual("File Not Found", exception.Message);
                    break;
                }

                i++;
            }
            Assert.AreEqual(rw.CountFiles(FilesContainer), 1);
        }
    }
}