using System;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.ComponentModel;
using System.Collections.Generic;
namespace XMLUtility.Models
{
    public class TestRun
    {
        /// <summary>
        /// Ent of the test run
        /// </summary>
        private DateTime startTime;
        /// <summary>
        /// Start Time of the test run
        /// </summary>
        private DateTime endTime;

        /// <summary>
        /// Maps name of test case with the test cases present.
        /// </summary>
        private int total = 0;

        private int passed = 0;

        private int failed = 0;

        private int inconclusive = 0;

        public Dictionary<string,TestCase> testList = new Dictionary<string, TestCase>();
        
        private string id;

        public int Total
        {
            get
            {
                return total;
            }
        }

        public int Passed
        {
            get
            {
                return passed;
            }
        }
        
        public int Failed
        {
            get
            {
                return failed;
            }
        }

        public int InConclusive 
        {
            get
            {
                return inconclusive;
            }
        }

        public DateTime StartDate
        {
            get
            {
                return startTime.Date;
            }
        }
        
        public DateTime EndDate
        {
            get
            {
                return endTime.Date;
            }
        }
        
        public DateTime StartTime
        {
            get
            {
                return startTime;
            }
            set
            {
                startTime = value;
            }
        }

        public DateTime EndTime
        {
            get
            {
                return endTime;
            }
            set
            {
                endTime = value;

            }
        }

        public string Id
        {
            get
            {
                return id;
            }
            set
            {
                id = value;
            }
        }

        public TestRun(DateTime start)
        {
            startTime = start;
        }
    
        public TestRun(string id, DateTime start)
        {
            startTime = start;
            Id = id;
        }


        /// <summary>
        /// Adds new test to the group
        /// </summary>
        public int AddTest(TestCase testCase)
        {
            if (!testList.ContainsKey(testCase.Name))
            {
                try
                {
                    testList.Add(testCase.Id, testCase);
                }
                catch (Exception e)
                {
                    return -1;
                }
                total++; // stores total number of test cases.
                switch (testCase.Result)
                {
                    case TestResult.Failed:
                        failed++; break;
                    case TestResult.Passed:
                        passed++;
                        break;
                    case TestResult.InConclusive:
                        inconclusive++;
                        break;
                }
                return 0;
            }
      
            //addition of new value failed.
            return -1;

        }

        /// <summary>
        /// Fetches result of test case.
        /// </summary>
        public TestResult GetTestResult(string testName)
        {
            if (testList.ContainsKey(testName))
            {
                return testList[testName].Result;
            }
            
            return TestResult.NoRun;
        }
    }
}
