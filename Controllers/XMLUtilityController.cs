using System;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.ComponentModel;
using System.Web.Mvc;
//using 


namespace XMLUtility.Controllers
{
/*    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;

    using DotNet.Highcharts.Enums;
    using DotNet.Highcharts.Helpers;
    using DotNet.Highcharts.Options;
   
    using XMLUtility.Models;

    public class XmlUtilityController : Controller
    {

        TrxDataStore DataStore = new TrxDataStore("TrxData");
        static TestRun selectedRun = null;
        //data to be shown on x-axis
        List<string> XData = new List<string>();
        List<object> yPassed = new List<Object>();
        List<object> yFailed = new List<object>();
        List<int> yNA = new List<int>();
        
        public List<string> xData
        {
            get
            {
                return (List<string>) Session["xData"];
            }
        }
        public TrxDataStore dataStore
        {
            get
            {
                return (TrxDataStore)this.Session["dataStore"];
            }
        }

        public void Populate(string path)
        {
            if(!dataStore.loaded)
            {
                dataStore.LoadData(path);
                dataStore.loaded = true;
                dataStore.ListOfDateTime.Sort();
            }
        }


        public DotNet.Highcharts.Highcharts InitializeChart()
        {
            DotNet.Highcharts.Highcharts chart = new DotNet.Highcharts.Highcharts("charts")
                .InitChart(new Chart { ZoomType = ZoomTypes.X, DefaultSeriesType = ChartTypes.Line })
                
                 .SetXAxis(new XAxis
                {Categories=xData.ToArray(),TickInterval= 20,Title =new XAxisTitle{Text="DateTime"}})
                 .SetYAxis(new YAxis{Min = 0,Title =new YAxisTitle{Text="Result"}})
                 .SetPlotOptions(new PlotOptions()
                 {Series=new PlotOptionsSeries
                    {
                        AllowPointSelect=true,
                        Point=new DotNet.Highcharts.Options.PlotOptionsSeriesPoint
                                  {
                                      Events=new PlotOptionsSeriesPointEvents{Click="LoadTestsForThisRun"}
                                  }}})
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
                
                            xmlhttp.open(" + "\"GET\",\"/XMLutility/AjaxTestList?dateTime=\"+this.category,true"+ "); xmlhttp.send();");
                  #endregion
            return chart;
        }


        public ActionResult Index(string path = "C:\\Users\\t-sumita\\Documents\\Visual Studio 2010\\Projects\\learning\\XMLUtility\\XMLUtility\\data\\transfer")
        {
            Session["dataStore"] = DataStore;
            Session["xData"] = XData;
            #region preprocessing 
            //dataStore.LoadData("C:\\Users\\sumit4ii\\Documents\\Visual Studio 2012\\Projects\\XMLUtility\\XMLUtility\\data\\transfer");
            //dataStore.LoadData("C:\\Users\\t-sumita\\Documents\\Visual Studio 2010\\Projects\\learning\\XMLUtility\\XMLUtility\\data\\transfer");
            this.Populate(path);
            foreach (DateTime v in dataStore.ListOfDateTime)
            {
                xData.Add(v.ToString());
                yPassed.Add(dataStore.testRunList[v].Passed);
                yFailed.Add(dataStore.testRunList[v].Failed);
                yNA.Add(dataStore.testRunList[v].InConclusive);
            }
            Series s1 = new Series{
                               Data = new Data(yFailed.ToArray()),
                               Name = "Failed",
                               Color = Color.Red
                           };
            Series s2 = new Series
                            {
                                Data = new Data(yPassed.ToArray()),Name = "Passed", Color= Color.Green
                            };
            List<Series> list = new List<Series>();

            list.Add(s1);
            list.Add(s2);
            #endregion

            #region ChartCreation
           
            DotNet.Highcharts.Highcharts chart = this.InitializeChart();
            chart.SetSeries(list.ToArray());
            #endregion
            
            return View(chart);
        }

        public string AjaxTestList(string dateTime)
        {
            
            DateTime time =  DateTime.Parse(dateTime);
            string a = "<b>"+time+"</b><br>";
            TestRun temp = dataStore.testRunList[time];
            foreach(TestCase test in temp.testList.Values)
            {
                a = a + "<a href='/XMLUtility/TestMonitor?testName="+test.Name+"'>" +test.Name + "&nbsp&nbsp&nbsp&nbsp&nbsp <b>"+ test.Result + "</b><br>";
            }
            return a;

        }
        public ActionResult TestMonitor(string testName)
        {
            #region preprocessing
            Dictionary<DateTime,TestResult> listOfResults = new Dictionary<DateTime, TestResult>();

            //result of test case testName
            foreach (TestRun v in dataStore.testRunList.Values)
            {
                listOfResults[v.StartTime] = v.testList[testName].Result;
            }

            List<DateTime> list = listOfResults.Keys.ToList();           
            list.Sort();

            List<string> xData = new List<string>();
            List<object> yData= new List<object>();
            foreach (DateTime time in list)
            {
                xData.Add(time.ToString());
            }
            foreach (DateTime time in list)
            {
                if(listOfResults[time] == TestResult.Passed)
                    yData.Add(1);
                else
                    yData.Add(0);
            }


            #endregion

            Series s1 = new Series { Data = new Data(yData.ToArray()) };
            List<Series> yAxis = new List<Series>();
            yAxis.Add(s1);
            DotNet.Highcharts.Highcharts chart = this.InitializeChart();

           chart.SetSeries(yAxis.ToArray()).SetTitle(new Title{Text = "Monitor Test"});
            return View(chart);
        }
        public ActionResult Range(string startDate, string endDate)
        {
            //dataStore.LoadData("C:\\Users\\t-sumita\\Documents\\Visual Studio 2010\\Projects\\learning\\XMLUtility\\XMLUtility\\data\\transfer");
           // dataStore.LoadData("C:\\Users\\sumit4ii\\Documents\\Visual Studio 2012\\Projects\\XMLUtility\\XMLUtility\\data\\transfer");

            try
            {
                if (DateTime.Parse(startDate) >= DateTime.Parse(endDate))
                {
                    ViewBag.Message = "End date should occur after the start date";
                    return this.View();
                }
            }
            catch (Exception e)
            {
                ViewBag.Message = "Error in parsing";
                return this.View();
            }
            foreach (DateTime v in dataStore.ListOfDateTime)
            {
                try
                {
                    if (v > DateTime.Parse(startDate) && v < DateTime.Parse(endDate))
                    {
                        xData.Add(v.ToString());
                        yPassed.Add(dataStore.testRunList[v].Passed);
                        yFailed.Add(dataStore.testRunList[v].Failed);
                        yNA.Add(dataStore.testRunList[v].InConclusive);
                    }

                }
                catch (Exception e)
                {
                    ViewBag.Message = "Error in parsing";
                    return this.View();                 
                }
            }
            Series s1 = new Series
            {
                Data = new Data(yFailed.ToArray()),
                Name = "Failed",
                Color = Color.Red
            };
            Series s2 = new Series
            {
                Data = new Data(yPassed.ToArray()),
                Name = "Passed",
                Color = Color.Green
            };
            List<Series> list = new List<Series>();

            list.Add(s1);
            list.Add(s2);

            DotNet.Highcharts.Highcharts chart = this.InitializeChart(); 
            chart.SetSeries(list.ToArray());

            return View(chart);
        }

        public void Reset()
        {
           
        }
    }*/
}
