using System.Net;
using System.Net.Sockets;

namespace IronMQUtils
{
    public static class IPAddressManager
    {
        public static string GetLocalIPAddress()
        {
            string hostAddress = "[Address N/A]";

            var hostAddresses = Dns.GetHostAddresses(Dns.GetHostName());
            foreach (IPAddress ipAddress in hostAddresses)
            {
                if (ipAddress.AddressFamily == AddressFamily.InterNetwork)
                {
                    hostAddress = ipAddress.ToString();
                }
            }

            return hostAddress;
        }
    }
}
