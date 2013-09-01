using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using XMLUtility.Models;
using DotNet.Highcharts;
using highOptions = DotNet.Highcharts.Options;
using DotNet.Highcharts.Options;
using DotNet.Highcharts.Helpers;
using DotNet.Highcharts.Enums;
using DotNet.Highcharts.Attributes;
using System.Collections;
using System.Drawing;
namespace XMLUtility.Controllers
{
    public class XMLUtilityV1Controller : Controller
    {
        //
        // GET: /XMLUtilityV1/

        #region properties

        private Dictionary<string, TrxDataStore> warehouse
        {
            get
            {
                return (Dictionary<string, TrxDataStore>)Session["warehouse"];
            }
            set
            {
                Session["warehouse"] = value;
            }
        }

        public Dictionary<string, TrxDataStore> Warehouse
        {
            get
            {
                if (warehouse == null) warehouse = new Dictionary<string, TrxDataStore>();
                return warehouse;
            }
            set
            {
                warehouse = value;
            }
        }

        private List<Series> storeData
        {
            get
            {
                return (List<Series>)Session["StoreData"];
            }
            set
            {
                Session["StoreData"] = value;
            }
        }

        public List<Series> StoreData
        {
            get
            {
                if (storeData == null)
                    storeData = new List<Series>();
                return storeData;
            }
            set
            {
                storeData = value;
            }
        }

        private List<Series> testData
        {
            get
            {
                return (List<Series>)Session["TestData"];
            }
            set
            {
                Session["TestData"] = value;
            }
        }

        public List<Series> TestData
        {
            get
            {
                if (testData == null)
                    testData = new List<Series>();
                return testData;
            }
            set
            {
                testData = value;
            }
        }

        EnvironmentSettings env = new EnvironmentSettings();

        #endregion


        public void LoadEnvironmentSettings(string key, string value)
        {
            env.Mappings["key"] = value;
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Index(string control, string param1 = "", string param2 = "", bool old = true, string start = "", string end = "")
        {            switch (control)
            {
                case "AddDataStore":
                    if (!String.IsNullOrEmpty(param1) && !String.IsNullOrEmpty(param2))
                    {
                        this.AddDataStore(param1, param2);
                        this.StoreMonitor(param1);
                    }
                    else
                    {
                        ViewBag.Message = "Id and Path are must";
                    }
                    break;
                case "TestMonitor":
                    if (!String.IsNullOrEmpty(param1) && !String.IsNullOrEmpty(param2))
                        this.TestMonitor(param1, param2, old);
                    else
                    {
                        ViewBag.Message = "TestID and StoreID are must";
                    }
                    break;
                case "StoreMonitor":
                    if (string.IsNullOrEmpty(param1))
                        this.StoreMonitor(param1, old);
                    else
                    {
                        ViewBag.Message = "StoreID can not be empty";
                    }
                    break;
                case "Reset":
                    this.Reset(param1);
                    break;
            }
            return View(this.Display());
        }

        public Highcharts Display()
        {
            Highcharts chart = this.InitializeChart();


            /*
             * Highcharts does not support addition series arrays. 
             * You are allowed to do this operation but chart will replace second list with the first one.
             * This can be useful but there are downsides for this like the current one, which forces us to take concat two lists to form
             * a single one which can be added to the highchart.
             */
            List<Series> union = new List<Series>();
            union.AddRange(StoreData);
            union.AddRange(TestData);

            chart.SetSeries(union.ToArray());
            return chart;
        }

        public double GetDateInJsFormat(DateTime date)
        {
            return (date - new DateTime(1970, 1, 1)).TotalMilliseconds;
        }

        public void AddDataStore(string id, string path)
        {
            TrxDataStore temp = new TrxDataStore(id);
            temp.LoadData(path);
            temp.loaded = true;
            try
            {
                Warehouse.Add(id, temp);

            }
            catch (Exception e)
            {
                ViewBag.ErrorMessage =
                    "This store is already added to the graph or name of store clashes with the existing store.";
            }
        }

        public void TestMonitor(string testId, string storeId, bool old = true)
        {
            if (!old)
            {
                TestData = new List<Series>();
            }
            highOptions.Point[] pt = new highOptions.Point[Warehouse[storeId].testRunList.Count];
            int i = 0;
            string testName = "";
            foreach (TestRun t in Warehouse[storeId].testRunList.Values)
            {
                testName = t.testList[testId].Name;
                pt[i] = new DotNet.Highcharts.Options.Point();
                pt[i].Id = t.testList[testId].Id;

                if (t.testList[testId].Result == TestResult.Failed)
                { pt[i].Y = 1; }
                else
                { pt[i].Y = 0; }

                pt[i].X = GetDateInJsFormat(t.StartTime);
                i++;
            }
            try
            {
                Array.Sort(pt, delegate(DotNet.Highcharts.Options.Point pt1, DotNet.Highcharts.Options.Point pt2)
                    {
                        return pt1.X.ToString().CompareTo(pt2.X.ToString());
                    }
                );
                TestData.Add(new Series { Data = new Data(pt), Name = testName + ":" + storeId, PlotOptionsLine = new PlotOptionsLine { AllowPointSelect = false, LineWidth = 2, ZIndex = -1 } });
            }
            catch (Exception e)
            {
                ViewBag.ErrorMessage = "Sorry, Error occurred";
            }
        }

        public void StoreMonitor(string storeId, bool old = true)
        {
            if (!old)
            {
                StoreData = new List<Series>();
            }
            if (String.IsNullOrEmpty(storeId))
            {
                StoreData = new List<Series>();
                foreach (TrxDataStore v in Warehouse.Values)
                {
                    DotNet.Highcharts.Options.Point[] pt = new DotNet.Highcharts.Options.Point[v.testRunList.Count];
                    int i = 0;
                    foreach (TestRun t in v.testRunList.Values)
                    {
                        pt[i] = new DotNet.Highcharts.Options.Point();
                        pt[i].Name = t.StartTime.ToString();
                        pt[i].Id = t.Id;
                        pt[i].Y = t.Failed;
                        pt[i].X = GetDateInJsFormat(t.StartTime);
                        i++;
                    }
                    try
                    {
                        Array.Sort(pt, delegate(DotNet.Highcharts.Options.Point pt1, DotNet.Highcharts.Options.Point pt2) 
                            { 
                                return pt1.X.ToString().CompareTo(pt2.X.ToString()); 
                            }
                            );
                        StoreData.Add(new Series { Data = new Data(pt), Name = v.Id });
                    }
                    catch (Exception e)
                    {
                        ViewBag.ErrorMessage = "Sorry, Error occurred";
                    }
                }
            }
            else
            {
                DotNet.Highcharts.Options.Point[] pt = new DotNet.Highcharts.Options.Point[Warehouse[storeId].testRunList.Count];
                int i = 0;
                foreach (TestRun testRun in Warehouse[storeId].testRunList.Values)
                {
                    pt[i] = new DotNet.Highcharts.Options.Point();
                    pt[i].Name = testRun.StartTime.ToString();
                    pt[i].Id = testRun.Id;
                    pt[i].Y = testRun.Failed;
                    pt[i].X = this.GetDateInJsFormat(testRun.StartTime);
                    i++;
                }
                try
                {
                    Array.Sort(pt, delegate(DotNet.Highcharts.Options.Point pt1, DotNet.Highcharts.Options.Point pt2)
                    {
                        return pt1.X.ToString().CompareTo(pt2.X.ToString());
                    }
                        );
                    StoreData.Add(new Series { Data = new Data(pt), Name = Warehouse[storeId].Id });
                }
                catch (Exception e)
                {
                    ViewBag.ErrorMessage = "Sorry, Error occurred";
                }
            }
            //Response.Redirect("/XMLUtilityV1/Display");
            //return this.Display();
        }

        public Highcharts InitializeChart()
        {
            Highcharts chart = new Highcharts("charts").InitChart(
                new Chart { ZoomType = ZoomTypes.X, DefaultSeriesType = ChartTypes.Line })
                                                       .SetPlotOptions(
                                                           new PlotOptions()
                                                               {
                                                                   Series =
                                                                       new PlotOptionsSeries
                                                                           {
                                                                               AllowPointSelect
                                                                                   =
                                                                                   true,
                                                                               Point =
                                                                                   new PlotOptionsSeriesPoint
                                                                                       {
                                                                                           Events
                                                                                               =
                                                                                               new PlotOptionsSeriesPointEvents
                                                                                                   {
                                                                                                       Click
                                                                                                           =
                                                                                                           "LoadTestsForThisRun"
                                                                                                   }
                                                                                       }
                                                                           }
                                                               })
                                                       .SetXAxis(new XAxis { Type = AxisTypes.Datetime })
            #region Ajax

.AddJavascripFunction(
                                                           "LoadTestsForThisRun", @"
                          var xmlhttp; 
                          if(window.XMLHttpRequest)
                            {
                                xmlhttp = new XMLHttpRequest();
                            } 
                
                            xmlhttp.onreadystatechange = function()
                            {
                                if(xmlhttp.readyState == 4 && xmlhttp.status == 200)
                                {
                                    document.getElementById('selectTest').innerHTML = xmlhttp.responseText; // the processing of showing new data goes here.
                                }
                            }
                
                            var point = this.point;                
      
            // alert(this.id);
                     xmlhttp.open(" + "\"GET\",\"/XMLutilityV1/AjaxTestList?runId=\"+this.id+\"&store=\"+this.series.name,true); xmlhttp.send(); "
                                           );
            /*  alert(" + "\"/XMLutilityV1/AjaxTestList?dateTime=\"+Highcharts.dateFormat('%e. %b %Y, %H:00', this.x)+\"&store=\"+this.series.name);*/
            #endregion

            return chart;
        }

        public string AjaxTestList(string runId, string store)
        {
            TestRun temp;
            string a = "";
            try
            {
                temp = Warehouse[store].testRunList[runId];
                a = "<b>" + temp.StartTime + "</b><br> <table>";
            }
            catch (Exception e)
            {
                a = "<b>This is a test case</b>";
                return a;
            }
            foreach (TestCase test in temp.testList.Values)
            {
                a = a + "<tr><td><a href='/XMLUtilityV1?control=TestMonitor&param1=" + test.Id + "&param2=" + store + "'>" + test.Name + "</td><td><b>" + test.Result + "</b></td></tr>";
            }
            return a;
        }

        public void Reset(string option = "ALL")
        {
            switch (option)
            {
                case "TEST":
                    TestData = new List<Series>();
                    break;
                case "STORE":
                    StoreData = new List<Series>();
                    break;
                default:
                    Warehouse = new Dictionary<string, TrxDataStore>();
                    StoreData = new List<Series>();
                    TestData = new List<Series>();
                    break;

            }
        }

    }
}
