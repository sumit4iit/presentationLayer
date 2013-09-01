using System;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.ComponentModel;
using System.Xml;
using System.IO;
namespace XMLUtility.Models
{
    using System.Collections.Generic;

    public class TrxDataStore : DataStore
    {
        //EnvironmentSettings env = new EnvironmentSettings("path");
        // the dates on which you can carry
        //public List<DateTime> ListOfDateTime =new List<DateTime>();
        public TrxDataStore(string Id)
            : base(Id)
        {
            id = Id;
        }

        public void LoadData(string path)
        {
            string[] list = Directory.GetFiles(path, "*.trx");
            foreach (var s in list)
            {
                this.TrxDataStoreReader(s);
            }
            loaded = true;
        }

        //assuming that this works. 
        //and it does, indeed !
        public void TrxDataStoreReader(string Path)
        {
            TestCase testCase = null;
            DateTime start = DateTime.Now;
            DateTime end = DateTime.Now;
            TestRun testRun = null;
            string testRunId = null;
            string testId = null;

            //path of trx file
            XmlReader reader = new XmlTextReader(Path);


            //read entire xml doc and search for elements with name unitTestResult
            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:

                        //for reading results
                        if (reader.Name == "UnitTestResult")
                        {
                            string testName = "";
                            testId = "";

                            while (reader.MoveToNextAttribute())
                            {
                                switch (reader.Name)
                                {
                                    case "testName":
                                        testName = reader.Value;
                                        break;
                                    case "outcome":
                                        if (reader.Value == "Passed") testCase = new TestCase(testName, TestResult.Passed, testId);
                                        else testCase = new TestCase(testName, TestResult.Failed, testId);
                                        break;
                                    case "testId":
                                        testId = reader.Value;
                                        break;
                                }
                            }
                            //adding test case to current testRun.
                            testRun.AddTest(testCase);
                        }
                        //Looks like start of new document lets create a test run.     
                        else if (reader.Name == "Times")
                        {
                            while (reader.MoveToNextAttribute())
                            {
                                switch (reader.Name)
                                {
                                    case "start":
                                        start = DateTime.Parse(reader.Value);
                                        string proper = start.ToString();
                                        start = DateTime.Parse(proper);
                                        break;
                                    case "finish":
                                        end = DateTime.Parse(reader.Value);
                                        break;
                                }
                            }
                            testRun = new TestRun(testRunId, start);
                            //list of date time....let the things look cooler
                            //there is no need of this. Remove this,
                            //but wait first check that this doesn't break any of the build.
                            //                            this.ListOfDateTime.Add(start);
                            testRun.EndTime = end;
                            //add this new test run to dictionary
                            this.AddTestRun(testRun);
/*                            if (!testRunList.ContainsKey(testRunId))
                                testRunList.Add(testRunId, testRun);*/
                        }
                        else if (reader.Name == "TestRun")
                        {
                            while (reader.MoveToNextAttribute())
                            {
                                switch (reader.Name)
                                {
                                    case "id":
                                        testRunId = reader.Value;
                                        break;
                                }
                            }
                        }
                        break;
                }
            }
        }
    }
}
