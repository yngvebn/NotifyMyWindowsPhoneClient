using System.Linq;
using System.Xml.Linq;

namespace NotifyMyWindowsPhoneClient
{
    public class NmwpResponse
    {
        private XDocument _xd;

        public NmwpResponse(XDocument xd)
        {
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
}