using System;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.ComponentModel;
using System.Collections.Generic;
namespace XMLUtility.Models
{
    public class Group
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
        private Dictionary<string,TestCase> testList;

        /// <summary>
        /// TimeStamp
        /// </summary>
        public DateTime StartDate
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }

        /// <summary>
        /// End Date.
        /// </summary>
        public DateTime EndDate
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }

        /// <summary>
        /// TimeStamp
        /// </summary>
        public DateTime StartTime
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }

        /// <summary>
        /// Start Date
        /// </summary>
        public DateTime EndTime
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }

        /// <summary>
        /// Adds new test to the group
        /// </summary>
        public void AddTest()
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Fetches result of test case.
        /// </summary>
        public void GetTest()
        {
            throw new System.NotImplementedException();
        }
        
        

    }
}
