using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using MVCwithADO.NET.Models.DALs;

namespace HomeWorkApp.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            // 使用SqlDataAdapter
            string conn = WebConfigurationManager.ConnectionStrings["Northwind"].ConnectionString.ToString();
            using (SqlDataAdapter adapter = new SqlDataAdapter("select * from Products", conn))
            {
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                return View(dt);
            }
           
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}