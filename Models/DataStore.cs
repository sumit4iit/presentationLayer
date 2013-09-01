using System;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.ComponentModel;

using System.Collections.Generic;


namespace XMLUtility.Models
{
    public class DataStore
    {
        /// <summary>
        /// Makes use of the fact that every group has a unique start time. Basically that how groups are defined.
        /// </summary>
        public Dictionary<string, TestRun> testRunList = new Dictionary<string, TestRun>();

        protected string id;

        public bool loaded = false;

        public DateTime RangeStart = DateTime.Now;

        public DateTime RangeEnd = new DateTime(1970,1,1); // very old date

        public DataStore(string Id)
        {
            id = Id;
        }

        public string Id
        {
            get
            {
                return id;
            }
        }
 
        public int AddTestRun(TestRun testRun)
        {
            if (!testRunList.ContainsKey(testRun.Id))
            {
                testRunList[testRun.Id] = testRun;
                if (testRun.StartTime < RangeStart)
                {
                    RangeStart = testRun.StartDate;
                }
                if (testRun.EndDate > RangeEnd)
                {
                    RangeEnd = testRun.EndDate.AddDays(1);
                }
                return 0;
            }
            else
                return -1;
        }
       
    }
}
