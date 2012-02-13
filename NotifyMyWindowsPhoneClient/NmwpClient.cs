using System;
using System.Collections.Generic;
using System.Linq;
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

    public static class HtmlHelpers
    {
        public static string HtmlEncode(this string s)
        {
            return System.Web.HttpUtility.HtmlEncode(s);
        }

        public static int SafeInt(this XElement x)
        {
            int i = 0;
            if (x == null) return i;
            int.TryParse(x.Value, out i);

            return i;
        }
        public static int SafeInt(this XAttribute x)
        {
            int i = 0;
            if (x == null) return i;
            int.TryParse(x.Value, out i);

            return i;
        }
    }
    public class NmwpResponse
    {
        private XDocument _xd;

        public NmwpResponse(XDocument xd)
        {
            // TODO: Complete member initialization
            this._xd = xd;
        }
        private bool IsValid
        {
            get
            {
                if (_xd.Root == null) return false;
                if( _xd.Root.Element("success") != null) return true;
                if (_xd.Root.Element("error") != null) return true;
                return false;
            }
        }
        public int Code
        {
            get
            {
                if (IsValid)
                    if (_xd.Root.Elements().First().Attribute("Code") != null)
                        return _xd.Root.Elements().First().Attribute("Code").SafeInt();
                return _xd.Root.Elements().First().Attribute("code").SafeInt();
            }
        }

        public string Message
        {
            get
            {
                return _xd.Root.Elements().First().Value;
            }
        }

        public bool Success
        {
            get
            {
                if (_xd.Root == null) return false;
                return _xd.Root.Element("success") != null;
            }
        }
        internal static NmwpResponse Create(XDocument xd)
        {
            return new NmwpResponse(xd);
        }
    }

    public class NmwpNotification
    {
        public string Application { get; private set; }
        public string Event { get; private set; }
        public string Description { get; private set; }
        public NmwpPriority Priority { get; private set; }

        public NmwpNotification(string application, string ev, string description, NmwpPriority priority = NmwpPriority.Normal)
        {
            Application = application;
            Event = ev;
            Description = description;
            Priority = priority;
        }
    }

    public enum NmwpPriority
    {
        Urgent = 2,
        Important = 1,
        Normal = 0,
        BelowNormal = -1,
        Unimportant = -2
    }
}
