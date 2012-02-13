using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Xml.Linq;

namespace NotifyMyWindowsPhoneClient
{
    public class NmwpClient
    {
        public Uri ServerUrl
        {
            get { return new Uri((_https ? "https://" : "http://") + "hal2010:8888"); }
        }


        private readonly string _apiKey;
        private readonly bool _https;

        private NmwpClient(string apiKey, bool https)
        {
            _apiKey = apiKey;
            _https = https;
        }

        public static NmwpClient Create(string apiKey, bool https = true)
        {
            return new NmwpClient(apiKey, https);
        }

        public NmwpResponse Notify(NmwpNotification notification)
        {
            WebRequest request = (HttpWebRequest)WebRequest.Create(new Uri(ServerUrl, "/publicapi/notify"));
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            using (var requestStream = request.GetRequestStream())
            {
                byte[] buffer = CreateBuffer(notification);
                requestStream.Write(buffer, 0, buffer.Length);
            }
            var response = request.GetResponse();
            if (response == null) throw new WebException("Something wrong happened");
            using(var responseStream = response.GetResponseStream())
            {
                  if (responseStream == null) throw new WebException("Something wrong happened");
          
                byte[] responseBytes = new byte[response.ContentLength];
                responseStream.Read(responseBytes, 0, (int)response.ContentLength);
                var xmlString = System.Text.Encoding.UTF8.GetString(responseBytes);
                XDocument xd = XDocument.Parse(xmlString);
                return NmwpResponse.Create(xd);
            }
        }

        private byte[] CreateBuffer(NmwpNotification notification)
        {
            string queryString = "apikey={4}&application={0}&event={1}&description={2}&priority={3}";
            var requestString = string.Format(queryString, notification.Application.HtmlEncode(),
                                              notification.Event.HtmlEncode(), notification.Description.HtmlEncode(),
                                              (int)notification.Priority, _apiKey);

            return System.Text.Encoding.UTF8.GetBytes(requestString);
        }
    }
}
