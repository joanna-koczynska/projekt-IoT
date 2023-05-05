using deviceLib;
using Microsoft.Azure.Devices.Client;

string deviceConnectionString = "HostName=IoT-centrum.azure-devices.net;DeviceId=Device1;SharedAccessKey=kFvAwD1b2RVkGhoPbH5ESa4a/Htxfn2PTnYPz+a85Ns=";

using var deviceClient = DeviceClient.CreateFromConnectionString(deviceConnectionString, TransportType.Mqtt);
await deviceClient.OpenAsync();
var device = new device(deviceClient);

/*await device.InitializeHandlers();*/


Console.WriteLine($"Connection success!");
/*
await device.SendMessages(10, 1000);*/

Console.WriteLine("Finished! Press Enter to close...");
Console.ReadLine();
