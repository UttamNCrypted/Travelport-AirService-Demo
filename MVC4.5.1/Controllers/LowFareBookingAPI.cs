using System;
using OFly.UniversalRecordService;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OFly.Utilities;
using System.Configuration;
using System.Xml.Serialization;

namespace OFly.Controllers
{
    class LowFareBookingAPI
    {
        public AirCreateReservationRsp AirBookingReservationRequest(OFly.AirService.AirPricingSolution airPricingSolution, OFly.AirService.AirItinerary airItinerary, string CommissionOrDiscount, string Amount, List<OFly.Models.BookingTravellerInfos> TravellerList, OFly.Models.FormOfPaymentInfo formOfPaymentInfo)
        {
            AirCreateReservationReq request = new AirCreateReservationReq();
            AirCreateReservationRsp airCreateReservationRsp = new AirCreateReservationRsp();

            try
            {

                OFly.AirService.AirPricingSolution aps = airPricingSolution;
                List<OFly.AirService.typeBaseAirSegment> segmentList = airItinerary.AirSegment.ToList();
                List<OFly.AirService.AirPricingInfo> pricingInfo = aps.AirPricingInfo.ToList();
                List<ActionStatus> actionStatus = new List<ActionStatus>();
                List<BookingTraveler> bookingTraveler = new List<BookingTraveler>();

                request.AuthorizedBy = "user";
                request.ProviderCode = segmentList[0].ProviderCode;
                request.RetainReservation = typeRetainReservation.Both;
                request.TargetBranch = OFly.Utilities.CommonUtility.GetConfigValue(OFly.Utilities.ProjectConstants.G_TARGET_BRANCH);

                BillingPointOfSaleInfo billSaleInfo = new BillingPointOfSaleInfo();
                billSaleInfo.OriginApplication = ConfigurationManager.AppSettings["OriginApplication"];
                request.BillingPointOfSaleInfo = billSaleInfo;

                AirPricingSolution apsu = new AirPricingSolution();

                //  converted SegmentList from AirService to UniversalRecordService
                string SegmentListXMLString = SegmentListConvertToXML(segmentList);
                List<typeBaseAirSegment> tbas = LoadFromXMLToSegmentList(SegmentListXMLString);


                //  converted PriceInfoList from AirService to UniversalRecordService
                string PricingInfoXMLString = PricingInfoListConvertToXML(pricingInfo);
                List<AirPricingInfo> api = LoadFromXMLToPricingInfo(PricingInfoXMLString);

                ////    need to add dynamic travelers as per textbox
                bookingTraveler = BookingTravelerList(TravellerList);
                request.BookingTraveler = bookingTraveler.ToArray();


                //apsu.AirItinerarySolutionRef
                apsu.AirPricingInfo = api.ToArray();
                //apsu.AirPricingResultMessage
                apsu.AirSegment = tbas.ToArray();
                apsu.AirSegmentRef = null;
                apsu.ApproximateBasePrice = aps.ApproximateBasePrice;
                apsu.ApproximateFees = aps.ApproximateFees;
                apsu.ApproximateTaxes = aps.ApproximateTaxes;
                apsu.ApproximateTotalPrice = aps.ApproximateTotalPrice;
                //apsu.AvailableSSR
                apsu.BasePrice = aps.BasePrice;
                apsu.CompleteItinerary = aps.CompleteItinerary;
                //apsu.Connection
                apsu.EquivalentBasePrice = aps.EquivalentBasePrice;
                apsu.FareNote = null;
                apsu.FareNoteRef = null;
                //apsu.FeeInfo
                apsu.Fees = aps.Fees;
                //apsu.HostToken
                //apsu.Itinerary
                apsu.ItinerarySpecified = aps.ItinerarySpecified;
                //apsu.Journey
                apsu.Key = aps.Key;
                //apsu.LegRef
                //apsu.MetaData
                //apsu.OptionalServices
                //apsu.PricingDetails
                apsu.QuoteDate = aps.QuoteDate;
                apsu.QuoteDateSpecified = aps.QuoteDateSpecified;
                apsu.Services = aps.Services;
                apsu.Taxes = aps.Taxes;
                //apsu.TaxInfo
                apsu.TotalPrice = aps.TotalPrice;


                request.AirPricingSolution = apsu;

                ////  Applying Commission/Discount if applicable.
                //if (CommissionOrDiscount != null && CommissionOrDiscount != "" && Amount != null && Amount != "")
                //{
                //    List<ManualFareAdjustment> fareAdjustmentList = new List<ManualFareAdjustment>();

                //    ManualFareAdjustment adjustment = new ManualFareAdjustment();

                //    if (CommissionOrDiscount == "Amount")
                //    {
                //        adjustment.AdjustmentType = typeAdjustmentType.Amount;
                //    }
                //    else
                //    {
                //        adjustment.AdjustmentType = typeAdjustmentType.Percentage;
                //    }

                //    adjustment.AppliedOn = typeAdjustmentTarget.Base;
                //    adjustment.Value = Convert.ToInt32(Amount);
                //    adjustment.PassengerRef = "1";
                //    fareAdjustmentList.Add(adjustment);

                //    api[0].AirPricingModifiers = new AirPricingModifiers()
                //    {
                //        ManualFareAdjustment = fareAdjustmentList.ToArray()
                //    };
                //}

                actionStatus.Add(new ActionStatus { Type = ActionStatusType.ACTIVE, TicketDate = "T*", ProviderCode = request.ProviderCode });
                request.ActionStatus = actionStatus.ToArray();

                request.FormOfPayment = AddFormOfPayment(formOfPaymentInfo);

                AirCreateReservationPortTypeClient client = new AirCreateReservationPortTypeClient("AirCreateReservationPort", OFly.Utilities.WsdlService.AIR_ENDPOINT);

                client.ClientCredentials.UserName.UserName = OFly.Utilities.Helper.RetrunUsername();
                client.ClientCredentials.UserName.Password = OFly.Utilities.Helper.ReturnPassword();


                var httpHeaders = OFly.Utilities.Helper.ReturnHttpHeader();
                client.Endpoint.EndpointBehaviors.Add(new HttpHeadersEndpointBehavior(httpHeaders));

                SupportedVersions supportedVersions = new SupportedVersions();
                supportedVersions.airVersion = "air_v31_0";
                airCreateReservationRsp = client.service(supportedVersions, request);

                return airCreateReservationRsp;
            }
            catch (Exception se)
            {
                throw se;
            }
        }
        private FormOfPayment[] AddFormOfPayment(OFly.Models.FormOfPaymentInfo formOfPaymentInfo)
        {
            List<FormOfPayment> payments = new List<FormOfPayment>();
            FormOfPayment fop = new FormOfPayment();
            //fop.Key = "jwt2mcK1Qp27I2xfpcCtAw==";//Key can be different
            fop.Type = "Credit";
            fop.Type = formOfPaymentInfo.IsCreditCard;
            CreditCard cc = new CreditCard()
            {
                BillingAddress = new typeStructuredAddress()
                {
                    AddressName = "Home",
                    Street = new string[] { formOfPaymentInfo.Address },
                    City = formOfPaymentInfo.City,
                    State = new State()
                    {
                        Value = formOfPaymentInfo.State
                    },
                    PostalCode = formOfPaymentInfo.PostalCode,
                    Country = formOfPaymentInfo.Country
                },
                ExpDate = formOfPaymentInfo.CardExpirationYear + "-" + formOfPaymentInfo.CardExpirationMonth,
                //Key = "GAJOYrVu4hGShsrlYIhwmw==",
                Number = formOfPaymentInfo.CardNumber,
                //BankCountryCode = "US",
                CVV = formOfPaymentInfo.CvvOrCvc,
                Type = "VI"
            };
            fop.Item = cc;
            payments.Add(fop);
            return payments.ToArray();
        }
        public UniversalRecordRetrieveRsp UniversalRecordRetrieveRequest(string LocatorCode)
        {
            try
            {
                UniversalRecordRetrieveReq request = new UniversalRecordRetrieveReq();
                UniversalRecordRetrieveRsp universalRecordRetrieveRsp = new UniversalRecordRetrieveRsp();

                request.AuthorizedBy = "user";
                request.TargetBranch = OFly.Utilities.CommonUtility.GetConfigValue(OFly.Utilities.ProjectConstants.G_TARGET_BRANCH);

                BillingPointOfSaleInfo billSaleInfo = new BillingPointOfSaleInfo();
                billSaleInfo.OriginApplication = ConfigurationManager.AppSettings["OriginApplication"];
                request.BillingPointOfSaleInfo = billSaleInfo;



                request.Item = LocatorCode;


                UniversalRecordRetrieveServicePortTypeClient client = new UniversalRecordRetrieveServicePortTypeClient("UniversalRecordRetrieveServicePort", OFly.Utilities.WsdlService.UNIVERSALRECORD_ENDPOINT);

                client.ClientCredentials.UserName.UserName = OFly.Utilities.Helper.RetrunUsername();
                client.ClientCredentials.UserName.Password = OFly.Utilities.Helper.ReturnPassword();


                var httpHeaders = OFly.Utilities.Helper.ReturnHttpHeader();
                client.Endpoint.EndpointBehaviors.Add(new HttpHeadersEndpointBehavior(httpHeaders));

                SupportedVersions supportedVersions = new SupportedVersions();
                supportedVersions.airVersion = "air_v31_0";
                supportedVersions.urVersion = "universal_v31_0";
                universalRecordRetrieveRsp = client.service(supportedVersions, request);

                return universalRecordRetrieveRsp;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static List<BookingTraveler> BookingTravelerList(List<OFly.Models.BookingTravellerInfos> travelerInfoList)
        {
            List<BookingTraveler> btList = new List<BookingTraveler>();
            int passengerCounter = 0;
            foreach (OFly.Models.BookingTravellerInfos bookingTravellerInfos in travelerInfoList)
            {
                BookingTraveler bt = new BookingTraveler { Key = bookingTravellerInfos.Key, TravelerType = bookingTravellerInfos.TravelerTypeCode, Gender = bookingTravellerInfos.Gender, DOBSpecified = true, DOB = Convert.ToDateTime(bookingTravellerInfos.DOBDay + "/" + bookingTravellerInfos.DOBMonth + "/" + bookingTravellerInfos.DOBYear) };
                BookingTravelerName btn = new BookingTravelerName { First = bookingTravellerInfos.FirstName, Last = bookingTravellerInfos.LastName, Middle = bookingTravellerInfos.MiddleName };

                List<PhoneNumber> phoneNumber = new List<PhoneNumber>();
                List<Email> email = new List<Email>();

                if (passengerCounter == 0)
                {
                    phoneNumber.Add(new PhoneNumber { CountryCode = bookingTravellerInfos.CountryCode, AreaCode = bookingTravellerInfos.AreaCode, Number = bookingTravellerInfos.Number });
                    bt.PhoneNumber = phoneNumber.ToArray();

                    email.Add(new Email { EmailID = bookingTravellerInfos.Email });
                    bt.Email = email.ToArray();
                }
                //else
                //{
                //    bt.PhoneNumber = btList[0].PhoneNumber;
                //    bt.Email = btList[0].Email;
                //}

                bt.BookingTravelerName = btn;

                btList.Add(bt);
                passengerCounter++;
            }

            //List<BookingTraveler> btList = new List<BookingTraveler>();

            //BookingTraveler bt = new BookingTraveler { Key = "1", TravelerType = "ADT", Age = "40", Gender = "M", Nationality = "IN" };
            //BookingTravelerName btn = new BookingTravelerName { First = "John", Last = "De", Middle = "A", Prefix = "Mr." };
            //List<typeStructuredAddress> address = new List<typeStructuredAddress>();

            //List<PhoneNumber> phoneNumber = new List<PhoneNumber>();
            //List<Email> email = new List<Email>();

            //phoneNumber.Add(new PhoneNumber { Location = "DEN", CountryCode = "1", AreaCode = "303", Number = "123456789" });
            //bt.PhoneNumber = phoneNumber.ToArray();

            //email.Add(new Email { EmailID = "jignesh.jinjariya@ncrypted.com" });
            //bt.Email = email.ToArray();

            //bt.BookingTravelerName = btn;

            //btList.Add(bt);

            return btList;
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

        #region Object Serialization Object to XML & XML to object
        public static string SegmentListConvertToXML(List<OFly.AirService.typeBaseAirSegment> segmentList)
        {
            try
            {

                var stringwriter = new System.IO.StringWriter();
                var serializer = new XmlSerializer(segmentList.GetType());
                serializer.Serialize(stringwriter, segmentList);
                return stringwriter.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static string PricingInfoListConvertToXML(List<OFly.AirService.AirPricingInfo> pricingInfo)
        {
            try
            {
                var stringwriter = new System.IO.StringWriter();
                var serializer = new XmlSerializer(pricingInfo.GetType());
                serializer.Serialize(stringwriter, pricingInfo);
                return stringwriter.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static string AirPricingSolutionConvertToXML(OFly.AirService.AirPricingSolution airPricingSolution)
        {
            try
            {

                var stringwriter = new System.IO.StringWriter();
                var serializer = new XmlSerializer(airPricingSolution.GetType());
                serializer.Serialize(stringwriter, airPricingSolution);
                return stringwriter.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static AirPricingSolution LoadFromXMLToAirPricingSolution(string xmlText)
        {
            try
            {
                var stringReader = new System.IO.StringReader(xmlText);
                var serializer = new XmlSerializer(typeof(AirPricingSolution));
                return serializer.Deserialize(stringReader) as AirPricingSolution;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static List<typeBaseAirSegment> LoadFromXMLToSegmentList(string xmlText)
        {
            try
            {
                var stringReader = new System.IO.StringReader(xmlText);
                var serializer = new XmlSerializer(typeof(List<typeBaseAirSegment>));
                return serializer.Deserialize(stringReader) as List<typeBaseAirSegment>;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static List<AirPricingInfo> LoadFromXMLToPricingInfo(string xmlText)
        {
            try
            {
                var stringReader = new System.IO.StringReader(xmlText);
                var serializer = new XmlSerializer(typeof(List<AirPricingInfo>));
                return serializer.Deserialize(stringReader) as List<AirPricingInfo>;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion Object Serialization Object to XML & XML to object
    }
}



        //[HttpPost]
        //public ActionResult FlightBooking(BookingPassengerInfoModel bookingPassengerInfoModel)
        //{
        //    try
        //    {
        //        if (Session["AirPriceRsp"] != null)
        //        {
        //            LowFareBookingAPI lowFareBookingAPI = new LowFareBookingAPI();
        //            AirPriceRsp airPriceRsp = (AirPriceRsp)Session["AirPriceRsp"];

        //            OFly.UniversalRecordService.AirCreateReservationRsp airCreateReservationRsp = lowFareBookingAPI.AirBookingReservationRequest(airPriceRsp.AirPriceResult[0].AirPricingSolution[0], airPriceRsp.AirItinerary, "", "", bookingPassengerInfoModel.BookingTravellerInfoData, bookingPassengerInfoModel.FormOfPaymentInfoData);

        //            Session["airCreateReservationRsp"] = airCreateReservationRsp;
        //        }

        //        return View("BookingConfirm");
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
