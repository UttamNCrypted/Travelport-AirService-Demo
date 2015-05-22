using MVC4._5._1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVC4._5._1.Controllers
{
    public class HomeController : Controller
    {
        TestingDbDataContext db = new TestingDbDataContext();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult SendSMS()
        {

            //  ViewBag.Message = "Modify this template to jump-start your ASP.NET MVC application.";

            string username = "jignesh.jinjariya@ncrypted.com";
            string password = "jignesh1";
            string msgsender = "Ncrypted Technologies";
            string destinationaddr = "+918866658130";
            string message = "Hello Uttam From Jignesh";

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

        public ActionResult TestSMSDeliveryReport(string password, string username, string refno, string mtstatus, string msgok, string errorcode, string now)
        {
            try
            {

                SMSTest sMSTest = new SMSTest();
                sMSTest.SMSTestId = Guid.NewGuid();
                sMSTest.UserPassword = password;
                sMSTest.UserName = username;
                sMSTest.RefNo = refno;
                sMSTest.MTStatus = mtstatus;
                sMSTest.MsgOk = msgok;
                sMSTest.ErrorCode = errorcode;
                sMSTest.SMSDateTime = now;
                sMSTest.CreatedDate = DateTime.Now;
                sMSTest.ModifiedDate = DateTime.Now;

                db.SMSTests.Add(sMSTest);
                db.SaveChanges();

            }
            catch (Exception ex)
            {
                SMSTest sMSTest = new SMSTest();
                sMSTest.SMSTestId = Guid.NewGuid();
                sMSTest.UserPassword = "Error Occoured" + password;
                sMSTest.UserName = username;
                sMSTest.RefNo = refno;
                sMSTest.MTStatus = mtstatus;
                sMSTest.MsgOk = msgok;
                sMSTest.ErrorCode = errorcode;
                sMSTest.SMSDateTime = now;
                sMSTest.CreatedDate = DateTime.Now;
                sMSTest.ModifiedDate = DateTime.Now;

                db.SMSTests.Add(sMSTest);
                db.SaveChanges();

                throw ex;
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
