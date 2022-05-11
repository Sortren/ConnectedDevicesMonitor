using SharpPcap;
using SharpPcap.LibPcap;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;

namespace ARPNetworkScannerModule
{
    public class ARPScanner
    {
        public static Dictionary<string, string> Resolve(IPAddress destinationIP, int msTimeout, LibPcapLiveDevice netInterface)
        {
            Dictionary<string, string> answeredHosts = new();

            ARP arpFrame = new ARP(netInterface);
            arpFrame.Timeout = TimeSpan.FromMilliseconds(msTimeout);

            var resolvedMacAddress = arpFrame.Resolve(destinationIP);

            if (resolvedMacAddress != null)
            {
                answeredHosts.Add(
                    resolvedMacAddress.ToString(), 
                    destinationIP.ToString()
                );
            }

            return answeredHosts;
        }

        public static Dictionary<string, string> Resolve(Subnet subnet, int msTimeout, LibPcapLiveDevice netInterface)
        {
            Dictionary<string, string> answeredHosts = new();

            ARP arpFrame = new ARP(netInterface);
            arpFrame.Timeout = TimeSpan.FromMilliseconds(msTimeout);

            IPAddress currentIP = subnet.IP;
            for (int i = 0; i < subnet.HostsNumber; i++)
            {
                currentIP = NetworkUtils.GetRelativeNextIPAddress(currentIP);

                var resolvedMacAddress = arpFrame.Resolve(currentIP);

                if (resolvedMacAddress != null)
                {
                    answeredHosts.Add(
                        resolvedMacAddress.ToString(), 
                        currentIP.ToString()
                    );
                }
            }

            return answeredHosts;
        }
    }
}
