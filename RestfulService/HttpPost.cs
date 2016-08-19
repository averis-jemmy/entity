using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;

namespace MyAverisClientTest
{
    public static class HttpGet
    {
        private static String url = "http://172.25.90.37/webapp/sendsms_soap.php";

        public static string HttpSubmit(string phoneNumber, string message)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Headers.Add("SOAPAction", string.Empty);
            request.ContentType = "text/xml;charset=\"utf-8\"";
            request.Accept = "text/xml";
            request.Method = "POST";

            string soapXml = "<soapenv:Envelope xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:urn=\"urn:apiwsdl\">" +
                           "<soapenv:Header/>" +
                           "<soapenv:Body>" +
                              "<urn:processAPI soapenv:encodingStyle=\"http://schemas.xmlsoap.org/soap/encoding/\">" +
                                 "<tar_num xsi:type=\"xsd:string\">+601137047578</tar_num>" +
                                 "<tar_msg xsi:type=\"xsd:string\">test</tar_msg>" +
                                 "<tar_mode xsi:type=\"xsd:string\">text</tar_mode>" +
                              "</urn:processAPI>" +
                           "</soapenv:Body>" +
                        "</soapenv:Envelope>";

            XmlDocument soapEnvelop = new XmlDocument();
            soapEnvelop.LoadXml(soapXml);

            using (Stream stream = request.GetRequestStream())
            {
                soapEnvelop.Save(stream);
            }

            try
            {
                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    if (response.Headers["ErrorMessage"] != null)
                    {
                        return response.Headers["ErrorMessage"] + " - " + response.Headers["ErrorDescription"];
                    }

                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        return String.Format("Request failed. Received HTTP {0}", response.StatusCode);
                    }

                    // grab the response
                    var responseValue = string.Empty;
                    using (var responseStream = response.GetResponseStream())
                    {
                        if (responseStream != null)
                        {
                            using (var reader = new StreamReader(responseStream))
                            {
                                responseValue += reader.ReadToEnd();
                            }
                        }
                    }

                    return responseValue;
                }
            }
            catch (WebException ex)
            {
                var responseValue = string.Empty;
                using (WebResponse response = ex.Response)
                {
                    HttpWebResponse httpResponse = (HttpWebResponse)response;
                    responseValue = httpResponse.StatusDescription;
                    using (Stream data = response.GetResponseStream())
                    {
                        using (var reader = new StreamReader(data))
                        {
                            responseValue += reader.ReadToEnd();
                            return responseValue;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
