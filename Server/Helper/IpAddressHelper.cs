using System;

namespace Tireless.Net.Mail.Helper
{
    static class IpAddressHelper
    {
        public static Boolean SubnetContainsIp(String network, Int32 netmask, String client)
        {
            //TODO: use IPAddress
            var octetsNetworkStr = network.Split('.');
            var octetsClientStr = client.Split('.');
            if (octetsClientStr.Length != 4 || octetsNetworkStr.Length != 4)
                throw new ArgumentException("ip not valid");

            var octetsNetwork = new Byte[octetsNetworkStr.Length];
            for (int i = 0; i < octetsNetworkStr.Length; i++)
                octetsNetwork[i] = Byte.Parse(octetsNetworkStr[i]);

            var octetsClient = new Byte[octetsClientStr.Length];
            for (int i = 0; i < octetsClientStr.Length; i++)
                octetsClient[i] = Byte.Parse(octetsClientStr[i]);

            for (int b = 0; b < 32 - netmask; b++)
            {
                Int32 i = octetsClient.Length - (b / 8) - 1;
                octetsClient[i] = (Byte)(octetsClient[i] & ~(uint)(1 << (b % 8)));
                octetsNetwork[i] = (Byte)(octetsNetwork[i] & ~(uint)(1 << (b % 8)));
            }

            for (int i = 0; i < octetsNetwork.Length; i++)
            {
                if (octetsNetwork[i] != octetsClient[i])
                    return false;
            }
            return true;
        }
    }
}
