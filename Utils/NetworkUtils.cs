using ConnectedDevicesMonitor.Exceptions;
using SharpPcap.LibPcap;
using System;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;

namespace ARPNetworkScannerModule
{
    class NetworkUtils
    {
        public static IPAddress GetRelativeNextIPAddress(IPAddress currentIPAddress, uint increment = 1)
        {
            byte[] addressBytes = currentIPAddress.GetAddressBytes().Reverse().ToArray();
            uint ipAsUint = BitConverter.ToUInt32(addressBytes, 0);
            byte[] nextAddress = BitConverter.GetBytes(ipAsUint + increment);

            string nextIPAddressAsStr = string.Join(".", nextAddress.Reverse());
            
            return IPAddress.Parse(nextIPAddressAsStr);
        }

        private static PhysicalAddress ConvertMacAddressToPhysicalAddress(string macAddress, string delimeter)
        {
            byte[] macAsByteArr = macAddress
                .Split(delimeter)
                .Select(x => Convert.ToByte(x, 16))
                .ToArray();


            return new PhysicalAddress(macAsByteArr);
        }

        public static LibPcapLiveDevice SelectNetInterfaceByMac(string macAddress)
        {
            PhysicalAddress physicalAddress = ConvertMacAddressToPhysicalAddress(macAddress, ":");
            
            var devices = LibPcapLiveDeviceList.Instance;

            return devices
                .FirstOrDefault(x => x.MacAddress != null && x.MacAddress.Equals(physicalAddress));
        }
    }
}

