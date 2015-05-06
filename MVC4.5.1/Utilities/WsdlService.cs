using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVC4._5._1.Utilities
{
    class WsdlService
    {
        //public static string URL_PREFIX = @"G:\jignesh\MVC4.5.1\MVC4.5.1";

        //public static string SYSTEM_WSDL = "Wsdl/System_v9_0/system.wsdl";
        //public static string AIR_WSDL = "Wsdl/Air_v29_0/air.wsdl";
        //public static string HOTEL_WSDL = "Wsdl/Hotel_v29_0/hotel.wsdl";
        //public static string VEHICLE_WSDL = "Wsdl/Vehicle_v29_0/vehicle.wsdl";
        //public static string UNIVERSAL_WSDL = "Wsdl/universal_v29_0/universal.wsdl";

        public static string ENDPOINT_PREFIX = "https://apac.universal-api.pp.travelport.com/B2BGateway/connect/uAPI/";        

        static public String SYSTEM_ENDPOINT = ENDPOINT_PREFIX + "SystemService";
        static public String AIR_ENDPOINT = ENDPOINT_PREFIX + "AirService";
        static public String HOTEL_ENDPOINT = ENDPOINT_PREFIX + "HotelService";
        static public String VEHICLE_ENDPOINT = ENDPOINT_PREFIX + "VehicleService";
    }
}
