using System;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.ComponentModel;

namespace XMLUtility.Models
{
    public enum TestResult
    {
        Passed,
        InConclusive,
        Failed,
        NoRun
    };
    public class TestCase
    {
        /// <summary>
        /// Name of the Test Case.
        /// </summary>
        private string name;
        /// <summary>
        /// Result of the test.`
        /// </summary>
        private TestResult result;
        private string id;

        /// <summary>
        /// Constructor
        /// </summary>
        public TestCase(string nameT, TestResult resultT, string id)
        {
            Id = id;
            Name = nameT;
            Result = resultT;
        }

        public TestCase(string nameT, TestResult resultT)
        {
            Name = nameT;
            Result = resultT;
        }

        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }

        public TestResult Result
        {
            get
            {
                return result;
            }
            set
            {
                result = value;
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
        
    }
}
