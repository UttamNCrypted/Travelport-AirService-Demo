using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVC4._5._1.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult SendSMS()
        {

            //  ViewBag.Message = "Modify this template to jump-start your ASP.NET MVC application.";

            string username = "uttam.nadiyapara@ncrypted.com";
            string password = "wj0yc";
            string msgsender = "Ncrypted Technologies";
            string destinationaddr = "+918866658130";
            string message = "Hello Uttam";

            ViaNettSMS s = new ViaNettSMS(username, password);
            ViaNettSMS.Result result;
            try
            {
                result = s.sendSMS(msgsender, destinationaddr, message);

                if (result.Success)
                {
                    //response.Write("Message successfully sent");
                    ViewBag.Message = "Message successfully sent.";
                }
                else
                {
                    //response.Write("Received error:" + result.ErrorCode + " " + result.ErrorMessage);
                    ViewBag.Message = "Received error:" + result.ErrorCode + " " + result.ErrorMessage;
                }
            }
            catch (System.Net.WebException ex)
            {
                //Error occurred while connecting to server.
                //response.Write(ex.Message);
                ViewBag.Message = ex.Message;
            }

            return View();
        }

        public ActionResult TestSMSDeliveryReport(string requesttype, int refno, int errorcode)
        {
            try
            {
                if (errorcode == 0)
                {
                    ViewBag.IsDelivered = "SMS Delivered Successfully!";
                }
                else
                {
                    ViewBag.IsDelivered = "SMS Delivered Unsuccessful!";
                }
                
            }
            catch (Exception ex)
            {
                ViewBag.IsDelivered = ex.Message;
            }
            return View();
        }

        public ActionResult Test()
        {
            try
            {
                AirSvcTest test = new AirSvcTest();
                test.Availability();

                return View();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}
