using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using XMLUtility.Models;

namespace XMLUtility.Controllers
{
    using DotNet.Highcharts.Options;

    public class TRXDatav1Controller : Controller
    {
        //
        // GET: /TRXData/


        // Data Store for different environmets
        public static Dictionary<string,TrxDataStore> dataStores = new Dictionary<string, TrxDataStore>();

        public List<Series> yData =  


        public ActionResult Index()
        {
            
            return View();
        }
        

        //this sml will allow you to specify creation of data stores and storing their paths. 
        public void LoadConfigFromXml()
        {
            
        }

        public void CreateDataStore(string name)
        {
            
        }


    }
}
