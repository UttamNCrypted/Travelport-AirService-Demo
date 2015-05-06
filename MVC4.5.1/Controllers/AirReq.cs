using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Travelport.ServiceProxy.KestrelData.AirProxy;

namespace MVC4._5._1.Controllers
{
    class AirReq
    {
        public static void AddPointOfSale(BaseSearchReq req, string appName)
        {
            BillingPointOfSaleInfo billSaleInfo = new BillingPointOfSaleInfo();
            billSaleInfo.OriginApplication = appName;
            req.BillingPointOfSaleInfo = billSaleInfo;
        }

        public static void AddPointOfSale(BaseCoreReq req, string appName)
        {
            BillingPointOfSaleInfo billSaleInfo = new BillingPointOfSaleInfo();
            billSaleInfo.OriginApplication = appName;
            req.BillingPointOfSaleInfo = billSaleInfo;
        }


        /**
         * Add the search passengers to the request.  We only add ADT (adult)
         * passengers and this only works for LowFareSearchReq objects.
         * @param request the req to add the passenger parameter to
         * @param n number of adults to put in the requset
         */

        public static void AddPassengers(BaseLowFareSearchReq request, int[] n)
        {
            List<SearchPassenger> passList = new List<SearchPassenger>();

            for (int i = 0; i < 3; i++)
            {
                int passenger = n[i];

                for (int j = 0; j < passenger; j++)
                {
                    if (i == 0)
                    {
                        SearchPassenger adult = new SearchPassenger();

                        adult.Code = "ADT";
                        adult.DOB = DateTime.Now.AddYears(-40);
                        adult.Age = "40";

                        passList.Add(adult);
                    }
                    else if (i == 1)
                    {
                        SearchPassenger child = new SearchPassenger();
                        child.Code = "CNN";
                        child.DOB = DateTime.Now.AddYears(-10);
                        child.Age = "10";
                        passList.Add(child);
                    }
                    else if (i == 2)
                    {
                        SearchPassenger infant = new SearchPassenger();
                        infant.Code = "INF";
                        infant.DOB = DateTime.Now.AddYears(-1);
                        infant.Age = "1";

                        passList.Add(infant);
                    }
                }
            }
            request.SearchPassenger = passList.ToArray();
        }

        public static AirPricingModifiers FareAdjustment(string AdjustmentType, decimal Amount)
        {
            AirPricingModifiers pricingModifiers = new AirPricingModifiers();
            List<ManualFareAdjustment> farelist = new List<ManualFareAdjustment>();
            ManualFareAdjustment manualFareAdjustment = new ManualFareAdjustment();

            if (AdjustmentType == "Amount")
            {
                manualFareAdjustment.AdjustmentType = typeAdjustmentType.Amount;
            }
            else
            {
                manualFareAdjustment.AdjustmentType = typeAdjustmentType.Percentage;
            }
            
            manualFareAdjustment.PassengerRef = "1";
            manualFareAdjustment.AppliedOn = typeAdjustmentTarget.Base;
            manualFareAdjustment.Value = Amount;

            farelist.Add(manualFareAdjustment);

            pricingModifiers.ManualFareAdjustment = farelist.ToArray();
            return pricingModifiers;
        }

        public static void AddAdultPassengers(BaseLowFareSearchReq request, int n)
        {
            List<SearchPassenger> passList = new List<SearchPassenger>();
            
            for (int i = 0; i < n; ++i)
            {
                SearchPassenger adult = new SearchPassenger();
                adult.Code = "ADT";
                adult.DOB = DateTime.Now.AddYears(-40);
                adult.Age = "40";                

                passList.Add(adult);
            }
            request.SearchPassenger = passList.ToArray();
        }

        public static void AddChildPassengers(BaseLowFareSearchReq request, int n)
        {
            List<SearchPassenger> passList = new List<SearchPassenger>();
            for (int i = 0; i < n; ++i)
            {
                SearchPassenger child = new SearchPassenger();
                child.Code = "CNN";
                child.DOB = DateTime.Now.AddYears(-10);
                child.Age = "10";
                passList.Add(child);
            }
            request.SearchPassenger = passList.ToArray();
        }

        public static void AddInfantPassengers(BaseLowFareSearchReq request, int n)
        {
            List<SearchPassenger> passList = new List<SearchPassenger>();
            for (int i = 0; i < n; ++i)
            {
                SearchPassenger infant = new SearchPassenger();
                infant.Code = "INF";
                infant.DOB = DateTime.Now.AddYears(-1);
                infant.Age = "1";

                passList.Add(infant);
            }
            request.SearchPassenger = passList.ToArray();
        }

        public SearchAirLeg CreateAirLeg(string originAirportCode, string destAirportCode)
        {
            typeSearchLocation originLoc = new typeSearchLocation();
            typeSearchLocation destLoc = new typeSearchLocation();

            // airport objects are just wrappers for their codes
            Airport origin = new Airport(), dest = new Airport();
            origin.Code = originAirportCode;
            dest.Code = destAirportCode;

            // search locations can be things other than airports but we are using
            // the airport version...
            originLoc.Item = origin;
            destLoc.Item = dest;

            return CreateLeg(originLoc, destLoc);
        }

        private SearchAirLeg CreateLeg(typeSearchLocation originLoc, typeSearchLocation destLoc)
        {
            SearchAirLeg leg = new SearchAirLeg();
            leg.SearchOrigin.SetValue(originLoc.Item, 0);
            leg.SearchDestination.SetValue(destLoc.Item, 0);

            return leg;
        }


        /**
         * Make a search location based on a city or airport code (city is 
         * preferred to airport in a conflict) and set the search radius to
         * 50mi.
        */
        public static typeSearchLocation CreateLocationNear(String cityOrAirportCode)
        {
            typeSearchLocation result = new typeSearchLocation();

            //city
            CityOrAirport place = new CityOrAirport();
            place.Code = cityOrAirportCode;
            place.PreferCity = true;
            result.Item = place;

            //distance
            Distance dist = new Distance();
            dist.Units = DistanceUnits.MI;
            dist.Value = string.Format("50");
            result.Distance = dist;

            return result;
        }


        /**
         * Mmodify a search leg to use economy class of service as preferred.
         * 
         * @param outbound the leg to modify
         */
        public static void AddEconomyPreferred(SearchAirLeg outbound)
        {
            AirLegModifiers modifiers = new AirLegModifiers();
            AirLegModifiersPreferredCabins cabins = new AirLegModifiersPreferredCabins();
            CabinClass cabinClass = new CabinClass();
            cabinClass.Type = "Economy";
            cabins.CabinClass = cabinClass;

            modifiers.PreferredCabins = cabins;
            outbound.AirLegModifiers = modifiers;
        }


        /**
         * Modify a search leg based on a departure date
         * 
         * @param outbound the leg to modify
         * @param departureDate the departure date in YYYY-MM-dd
         */
        public static void AddDepartureDate(SearchAirLeg outbound, String departureDate)
        {
            // flexible time spec is flexible in that it allows you to say
            // days before or days after
            typeFlexibleTimeSpec noFlex = new typeFlexibleTimeSpec();
            noFlex.PreferredTime = departureDate;

            List<typeFlexibleTimeSpec> flexList = new List<typeFlexibleTimeSpec>();
            flexList.Add(noFlex);
            outbound.Items = flexList.ToArray();

        }


        /**
        * Search modifiers to create, usually a GDS code plus optionally 
        * RCH (Helper.RAIL_PROVIDER) or ACH (Helper.LOW_COST_PROVIDER).
        * 
        * @param providerCode  one or more provider codes (zero will not work!)
        * @return the modifiers object
        */
        public static AirSearchModifiers CreateModifiersWithProviders(String[] providerCode)
        {
            AirSearchModifiers modifiers = new AirSearchModifiers();
            List<Provider> providers = new List<Provider>();

            for (int i = 0; i < providerCode.Length; ++i)
            {
                Provider p = new Provider();
                // set the code for the provider
                p.Code = providerCode[i];
                // can be many providers, but we just use one
                providers.Add(p);
            }
            modifiers.PreferredProviders = providers.ToArray();
            return modifiers;
        }

        //public static SearchAirLeg CreateSearchLeg(String originAirportCode, String destAirportCode)
        //{
        //    // TODO Auto-generated method stub
        //    typeSearchLocation originLoc = new typeSearchLocation();
        //    typeSearchLocation destLoc = new typeSearchLocation();

        //    // airport objects are just wrappers for their codes
        //    Airport origin = new Airport(), dest = new Airport();
        //    origin.Code = originAirportCode;
        //    dest.Code = destAirportCode;

        //    // search locations can be things other than airports but we are using
        //    // the airport version...
        //    originLoc.Item = origin;
        //    destLoc.Item = dest;

        //    return CreateSearchLeg(originLoc, destLoc);
        //}

        public static SearchAirLeg CreateSearchLeg(String originAirportCode, String destAirportCode)
        {
            
            // TODO Auto-generated method stub
            typeSearchLocation originLoc = new typeSearchLocation();
            typeSearchLocation destLoc = new typeSearchLocation();

            CityOrAirport origin = new CityOrAirport();
            CityOrAirport dest = new CityOrAirport();

            origin.Code = originAirportCode;
            origin.PreferCity = true;

            dest.Code = destAirportCode;
            dest.PreferCity = true;
            
            //// airport objects are just wrappers for their codes
            //Airport origin = new Airport(), dest = new Airport();
            //origin.Code = originAirportCode;
            //dest.Code = destAirportCode;
            
            // search locations can be things other than airports but we are using
            // the airport version...
            originLoc.Item = origin;
            destLoc.Item = dest;

            return CreateSearchLeg(originLoc, destLoc);
        }

        private static SearchAirLeg CreateSearchLeg(typeSearchLocation originLoc,
                typeSearchLocation destLoc)
        {
            SearchAirLeg leg = new SearchAirLeg();

            leg.SearchOrigin = new typeSearchLocation[1];
            leg.SearchDestination = new typeSearchLocation[1];
            leg.SearchOrigin.SetValue(originLoc, 0);
            leg.SearchDestination.SetValue(destLoc, 0);

            return leg;
        }

        public static void AddSearchDepartureDate(SearchAirLeg outbound,
                String departureDate)
        {
            // flexible time spec is flexible in that it allows you to say
            // days before or days after
            typeFlexibleTimeSpec noFlex = new typeFlexibleTimeSpec();
            noFlex.PreferredTime = departureDate;

            List<typeFlexibleTimeSpec> flexList = new List<typeFlexibleTimeSpec>();
            flexList.Add(noFlex);
            outbound.Items = flexList.ToArray();
        }

        public static void AddSearchEconomyPreferred(SearchAirLeg outbound, string cabinClassName)
        {
            AirLegModifiers modifiers = new AirLegModifiers();
            AirLegModifiersPreferredCabins cabins = new AirLegModifiersPreferredCabins();
            CabinClass cabinClass = new CabinClass();
            cabinClass.Type = cabinClassName;
            cabins.CabinClass = cabinClass;

            modifiers.PreferredCabins = cabins;
            outbound.AirLegModifiers = modifiers;
        }

    }
}
