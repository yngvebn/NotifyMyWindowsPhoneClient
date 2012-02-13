using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NotifyMyWindowsPhoneClient;

namespace Test
{
    public class Program
    {
        public const string ApiKey = "000000000000000000000000000000000000000000000000";

        static void Main(string[] args)
        {
            NmwpClient client = NmwpClient.Create(ApiKey, false);
            var response = client.Notify(new NmwpNotification("Test application", "Testing", "Testing this magnificent console app",
                                               NmwpPriority.Urgent));

            if (!response.Success)
                Console.Write("Failed: " + response.Message);
            else
                Console.Write("Success!");
            Console.Read();

        }
    }
}
