using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MyAverisClientTest
{
    public enum HttpVerb
    {
        GET,
        POST,
        PUT,
        DELETE
    }

    public class ContentTypeString
    {
        private ContentTypeString(string value) { Value = value; }

        public string Value { get; set; }

        public static ContentTypeString XML { get { return new ContentTypeString("text/xml"); } }
        public static ContentTypeString JSON { get { return new ContentTypeString("application/json"); } }
    }

    public class RestClient
    {

        public string EndPoint { get; set; }
        public HttpVerb Method { get; set; }
        public string ContentType { get; set; }
        public string PostData { get; set; }

        public RestClient(string endpoint)
        {
            EndPoint = endpoint;
            Method = HttpVerb.GET;
            ContentType = ContentTypeString.XML.Value;
            PostData = "";
        }

        public RestClient(string endpoint, HttpVerb method, ContentTypeString contentType, string postData)
        {
            EndPoint = endpoint;
            Method = method;
            ContentType = contentType.Value;
            PostData = postData;
        }

        public string ProcessRequest(string parameters, List<KeyValuePair<string, string>> headers, byte[] postData)
        {
            var request = (HttpWebRequest)WebRequest.Create(EndPoint + parameters);
            request.Method = Method.ToString();
            request.ContentLength = 0;
            request.ContentType = "text/plain";
            request.KeepAlive = false;
            if (headers != null)
            {
                foreach (KeyValuePair<string, string> item in headers)
                {
                    request.Headers.Add(item.Key, item.Value);
                }
            }

            try
            {
                if (postData != null && (Method == HttpVerb.POST || Method == HttpVerb.PUT))
                {
                    byte[] fileToSend = postData;
                    request.ContentLength = fileToSend.Length;

                    using (Stream requestStream = request.GetRequestStream())
                    {
                        // Send the file as body request.
                        requestStream.Write(fileToSend, 0, fileToSend.Length);
                        requestStream.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
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

        public string ProcessRequest(string parameters, List<KeyValuePair<string, string>> headers, bool isStream)
        {
            var request = (HttpWebRequest)WebRequest.Create(EndPoint + parameters);
            request.Method = Method.ToString();
            request.ContentLength = 0;
            request.ContentType = ContentType;
            request.KeepAlive = false;
            if (headers != null)
            {
                foreach (KeyValuePair<string, string> item in headers)
                {
                    request.Headers.Add(item.Key, item.Value);
                }
            }

            try
            {
                if (!string.IsNullOrEmpty(PostData) && (Method == HttpVerb.POST || Method == HttpVerb.PUT))
                {
                    var bytes = Encoding.GetEncoding("iso-8859-1").GetBytes(PostData);
                    if (ContentType.ToUpper().Contains("JSON"))
                        bytes = Encoding.UTF8.GetBytes(PostData);

                    request.ContentLength = bytes.Length;

                    using (var writeStream = request.GetRequestStream())
                    {
                        writeStream.Write(bytes, 0, bytes.Length);
                    }
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
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
                            if (!isStream)
                            {
                                using (var reader = new StreamReader(responseStream))
                                {
                                    responseValue += reader.ReadToEnd();
                                }
                            }
                            else
                            {
                                byte[] buffer = new byte[32768];
                                MemoryStream ms = new MemoryStream();
                                int bytesRead, totalBytesRead = 0;
                                do
                                {
                                    bytesRead = responseStream.Read(buffer, 0, buffer.Length);
                                    totalBytesRead += bytesRead;

                                    ms.Write(buffer, 0, bytesRead);
                                } while (bytesRead > 0);

                                responseValue += Convert.ToBase64String(ms.ToArray());
                                ms.Close();
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
