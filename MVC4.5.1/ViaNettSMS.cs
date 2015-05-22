using System.Web; // NB! need to add System.Web as a reference
using System.Net;

public class ViaNettSMS
{
    private string username;
    private string password;

    public ViaNettSMS(string username, string password)
    {
        this.username = username;
        this.password = password;
    }
    public Result sendSMS(string msgsender, string destinationaddr, string message)
    {
        
        string url;
        string serverResult;
        long l;
        Result result;

        // Build the URL request for sending SMS.
        url = "http://smsc.vianett.no/ActiveServer/MT/?"
            + "username=" + HttpUtility.UrlEncode(username) 
            + "&password=" + HttpUtility.UrlEncode(password) 
            + "&destinationaddr=" + HttpUtility.UrlEncode(destinationaddr, System.Text.Encoding.GetEncoding("ISO-8859-1")) 
            + "&message=" + HttpUtility.UrlEncode(message, System.Text.Encoding.GetEncoding("ISO-8859-1")) 
            + "&refno=1";

        // Check if the message sender is numeric or alphanumeric.
        if (long.TryParse(msgsender, out l))
        {
            url = url + "&sourceAddr=" + msgsender;
        }
        else
        {
            url = url + "&fromAlpha=" + msgsender;
        }
        // Send the SMS by submitting the URL request to the server. The response is saved as an XML string.
        serverResult = DownloadString(url);
        // Converts the XML response from the server into a more structured Result object.
        result = ParseServerResult(serverResult);
        // Return the Result object.
        return result;
    }
   
    private string DownloadString(string URL)
    {
        using (System.Net.WebClient wlc = new System.Net.WebClient())
        {
            // Create WebClient instanse.
            try
            {
                // Download and return the xml response
                return wlc.DownloadString(URL);
            }
            catch (WebException ex)
            {
                // Failed to connect to server. Throw an exception with a customized text.
                throw new WebException("Error occurred while connecting to server. " + ex.Message, ex);
            }
        }
    }

    
   
    private Result ParseServerResult(string ServerResult)
    {
        System.Xml.XmlDocument xDoc = new System.Xml.XmlDocument();
        System.Xml.XmlNode ack;
        Result result = new Result();
        xDoc.LoadXml(ServerResult);
        ack = xDoc.GetElementsByTagName("ack")[0];
        result.ErrorCode = int.Parse(ack.Attributes["errorcode"].Value);
        result.ErrorMessage = ack.InnerText;
        result.Success = (result.ErrorCode == 0);
        return result;
    }

    public class Result
    {
        public bool Success;
        public int ErrorCode;
        public string ErrorMessage;
    }
}