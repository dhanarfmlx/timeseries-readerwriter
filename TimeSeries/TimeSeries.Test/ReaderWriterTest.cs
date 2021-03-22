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
            DateTime startDate = new DateTime(2021, 3, 15);
            DateTime finishDate = new DateTime(2021, 3, 16);
            DateTime whenReadFile = new DateTime(2021, 3, 15, 12, 0, 0); 
            DateTime whichDateToRead = new DateTime(2021, 3, 15);

            string readResult = rw.runSimulation(startDate, finishDate, whenReadFile, whichDateToRead,FilesContainer,step);

            Assert.AreEqual(readResult.Length < 4000000, true);
            Assert.AreEqual(rw.CountFiles(FilesContainer), 2);
            Assert.AreEqual(rw.deleteWeekly(FilesContainer), false);
        }
        
        [Test]
        [TestCase(1)]
        public void Test2(int step)
        {
            DateTime startDate = new DateTime(2021, 3, 15);
            DateTime finishDate = new DateTime(2021, 3, 16);
            DateTime whenReadFile = new DateTime(2021, 3, 15, 12, 0, 0);
            DateTime whichDateToRead = new DateTime(2022, 3, 15);

            string readResult = rw.runSimulation(startDate, finishDate, whenReadFile, whichDateToRead, FilesContainer, step);

            Assert.AreEqual("File Not Found", readResult);
            Assert.AreEqual(rw.CountFiles(FilesContainer), 2);
            Assert.AreEqual(rw.deleteWeekly(FilesContainer), false);
        }

        [Test]
        [TestCase(1)]
        public void Test3(int step)
        {
            DateTime startDate = new DateTime(2021, 3, 15);
            DateTime finishDate = new DateTime(2021, 3, 19);
            DateTime whenReadFile = new DateTime(2021, 3, 18);
            DateTime whichDateToRead = new DateTime(2021, 3, 15);

            string readResult = rw.runSimulation(startDate, finishDate, whenReadFile, whichDateToRead, FilesContainer, step);

            Assert.AreEqual(readResult.Length > 7000000, true);
            Assert.AreEqual(rw.CountFiles(FilesContainer), 5);
            Assert.AreEqual(rw.deleteWeekly(FilesContainer), false);
        }

        [Test]
        [TestCase(1)]
        public void Test4(int step)
        {
            DateTime startDate = new DateTime(2021, 3, 18);
            DateTime finishDate = new DateTime(2021, 3, 26);
            DateTime whenReadFile = new DateTime(2021, 3, 19);
            DateTime whichDateToRead = new DateTime(2021, 3, 18);

            string readResult = rw.runSimulation(startDate, finishDate, whenReadFile, whichDateToRead, FilesContainer, step);

            Assert.AreEqual(readResult.Length > 7000000, true);
            Assert.AreEqual(rw.CountFiles(FilesContainer), 2);
        }
    }
}