# ConnectedDevicesMonitor
#### Service based on .NET Worker, allows the client to monitor the devices that are connected to the local network

### Technologies used:

<img align = "left" alt = "C#" width = "26px" src = "https://user-images.githubusercontent.com/79079000/167954761-40baeae8-1e6c-4d1d-ac9e-f256da9098b2.jpg" />
<img align = "left" alt = ".NET" width = "26px" src = "https://user-images.githubusercontent.com/79079000/167954911-6fe3a336-44d2-4462-a1e1-79577512b7c1.png" />
<img align = "left" alt = "Sqlite" width = "26px" src = "https://user-images.githubusercontent.com/79079000/167956158-ff2d894d-d7e7-45b1-b016-558f15ea0d8d.png" />


<br />

----

### How does it work in general?

Every x minutes defined by the client, the whole local network will be scanned using ARP protocol.
The program sends an ARP frame to the subnet broadcast address, and starts to listen for the hosts to answer the frame.
Each host from the network will present to the service as an IP Address and corresponding MAC Address. Having those two we can easily define connected device to 
the network. The app will receive neccessary information and save it to the database. Also if a connected device turns off but it was present in the database
in previous scan, the service will change the device state to inactive in database. Accordingly, if the device will show up in next scan but previously it was in
inactive state - it's going to be changed to active once again. It prevents from overloading the database. 

----

### Configuration

```json
  "ConnectedDevicesMonitorSettings": {
    "NetworkConfig": {
      "IPAddress": "192.168.0.0",
      "CIDRMask": 24,
      "NetworkInterfaceMacAddress": "AA:AA:AA:AA:AA:AA"
    },
    "ScannerConfig": {
      "ScanEveryXMinutes": 5,
      "ScanTimeoutMs": 150
    }
  }
```

In appsettings.json we have some following variables: <br />
IPAddress -> the IP Address of your local network subnet <br />
CIDRMask -> the mask in CIDR format of your subnet <br />
NetworkInterfaceMacAddress -> MAC address of your network interface from which scans are going to be made <br />
ScanEveryXMinutes -> defines the period of scans <br />
ScanTimeoutMs -> preferably don't change it below 100/150 unless you have good router (like mikrotik or cisco) <br />

----

### Deployment as a Windows Service
This service is meant to be deployed as a Windows service. It can be easily built via tools provided by Visual Studio (check this out on the internet).
