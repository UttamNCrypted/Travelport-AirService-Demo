using Travelport.ServiceProxy.KestrelData.AirProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;

namespace MVC4._5._1.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
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

        #region Flight Search Module

        public ActionResult Test()
        {
            try
            {
                string from = "RAJ";
                string to = "DXB";
                string departureDate = "2015-05-10";
                string returnDate = "2015-05-15";
                string trip = "RoundTrip";
                string adult = "1";
                string child = "0";
                string infant = "0";
                string cabin = "Economy";

                LowFareSearchRsp lowFareSearchRsp = LowFareAvailability(from, to, departureDate, returnDate, trip, adult, child, infant, cabin);
                
                
                Session["AvailabilitySearchResponse"] = lowFareSearchRsp;

                return View();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public LowFareSearchRsp LowFareAvailability(string from, string to, string departureDate, string returnDate, string trip, string adult, string child, string infant, string cabin)
        {
            LowFareSearchReq request = new LowFareSearchReq();
            LowFareSearchRsp rsp;

            request = LowFareSetupRequestForSearch(request, from, to, departureDate, returnDate, trip, adult, child, infant, cabin);


            AirLowFareSearchPortTypeClient client = new AirLowFareSearchPortTypeClient("AirLowFareSearchPort", MVC4._5._1.Utilities.WsdlService.AIR_ENDPOINT);
            client.ClientCredentials.UserName.UserName = MVC4._5._1.Utilities.Helper.RetrunUsername();
            client.ClientCredentials.UserName.Password = MVC4._5._1.Utilities.Helper.ReturnPassword();

            try
            {
                var httpHeaders = MVC4._5._1.Utilities.Helper.ReturnHttpHeader();
                client.Endpoint.EndpointBehaviors.Add(new HttpHeadersEndpointBehavior(httpHeaders));

                rsp = client.service(request);

                return rsp;
            }
            catch (Exception se)
            {
                throw se;
            }
        }

        private LowFareSearchReq LowFareSetupRequestForSearch(LowFareSearchReq request, string from, string to, string departureDate, string returnDate, string trip, string adult, string child, string infant, string cabin)
        {
            //add in the tport branch code
            request.TargetBranch = MVC4._5._1.Utilities.CommonUtility.GetConfigValue(MVC4._5._1.Utilities.ProjectConstants.G_TARGET_BRANCH);
            request.SolutionResult = true;

            AirPricingModifiers pricingModifiers = new AirPricingModifiers();

            if (trip == "RoundTrip")
            {
                pricingModifiers.OneWayShop = false;
                request.AirPricingModifiers = pricingModifiers;
            }
            else
            {
                pricingModifiers.OneWayShop = true;
                request.AirPricingModifiers = pricingModifiers;
            }


            //  I have applied for Commision of 40, but won't get modified Base Price.

            //pricingModifiers = AirReq.FareAdjustment("Amount", +40);
            //request.AirPricingModifiers = pricingModifiers;

            //set the GDS via a search modifier
            String[] gds = new String[] { ConfigurationManager.AppSettings["ProviderCode"] };
            AirSearchModifiers modifiers = AirReq.CreateModifiersWithProviders(gds);


            AirReq.AddPointOfSale(request, ConfigurationManager.AppSettings["OriginApplication"]);

            int[] passengers = new int[3];
            passengers[0] = Convert.ToInt32(adult);
            passengers[1] = Convert.ToInt32(child);
            passengers[2] = Convert.ToInt32(infant);

            AirReq.AddPassengers(request, passengers);

            SearchAirLeg outbound = AirReq.CreateSearchLeg(from, to);
            AirReq.AddSearchDepartureDate(outbound, departureDate);

            AirReq.AddSearchEconomyPreferred(outbound, cabin);
            
            //coming back
            if (trip == "RoundTrip")
            {
                SearchAirLeg ret = AirReq.CreateSearchLeg(to, from);
                AirReq.AddSearchDepartureDate(ret, returnDate);
                AirReq.AddSearchEconomyPreferred(ret, cabin);

                request.Items = new SearchAirLeg[2];
                request.Items.SetValue(outbound, 0);
                request.Items.SetValue(ret, 1);
            }
            else
            {
                request.Items = new SearchAirLeg[1];
                request.Items.SetValue(outbound, 0);
            }

            return request;
        }
        
        #endregion Flight Search Module
    }
}
