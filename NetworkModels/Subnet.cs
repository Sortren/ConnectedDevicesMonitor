using System;
using System.Linq;
using System.Net;


namespace ARPNetworkScannerModule
{
    public class Subnet
    {
        public IPAddress IP { get; set; }
        public int CIDRMask { get; set; }
        public uint UintMask { get; }
        public int HostsNumber { get; }
        public string BroadcastIPAddress { get; }

        public Subnet(string ipAddress, int cidrMask)
        {
            IP = IPAddress.Parse(ipAddress);
            CIDRMask = cidrMask;
            HostsNumber = CalculateHostsNumber();
            UintMask = ConvertCIDRMaskToUInt32();
            BroadcastIPAddress = CalculateBroadcastIP();
        }

        private int CalculateHostsNumber()
        {
            return (int)Math.Pow(2, 32 - CIDRMask) - 2;
        }

        private string CalculateBroadcastIP()
        {
            byte[] addressBytes = IP.GetAddressBytes().Reverse().ToArray();
            uint ipAsUInt = BitConverter.ToUInt32(addressBytes, 0);
            uint broadcastAsUInt = ipAsUInt | ~UintMask;

            byte[] broadcastAsBytes = BitConverter.GetBytes(broadcastAsUInt);

            return string.Join(".", broadcastAsBytes.Reverse());
        }

        private uint ConvertCIDRMaskToUInt32()
        {
            int[] binaryMaskAsArr = new int[32];
            for (int i = 0; i < CIDRMask; i++)
            {
                binaryMaskAsArr[i] = 1;
            }

            string binaryMaskAsStr = string.Join("", binaryMaskAsArr);
            uint binaryMask = Convert.ToUInt32(binaryMaskAsStr, 2);

            return binaryMask;
        }

    }
}
