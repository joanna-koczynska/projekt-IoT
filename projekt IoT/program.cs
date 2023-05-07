using deviceLib;
using Microsoft.Azure.Devices.Client;
using Opc.UaFx.Client;
using Opc.UaFx;

    string deviceConnectionString = "HostName=IoT-centrum.azure-devices.net;DeviceId=Device1;SharedAccessKey=kFvAwD1b2RVkGhoPbH5ESa4a/Htxfn2PTnYPz+a85Ns=";

using var deviceClient = DeviceClient.CreateFromConnectionString(deviceConnectionString, TransportType.Mqtt);
await deviceClient.OpenAsync();
    var device = new functions(deviceClient);

    Console.WriteLine($"Connection success!");

    using (var client = new OpcClient("opc.tcp://localhost:4840/"))
    {
        client.Connect();


        OpcValue WorkorderId = client.ReadNode("ns=2;s=Device 1/WorkorderId");
        OpcValue ProductionStatus = client.ReadNode("ns=2;s=Device 1/ProductionStatus");
        OpcValue GoodCount = client.ReadNode("ns=2;s=Device 1/GoodCount");
        OpcValue BadCount = client.ReadNode("ns=2;s=Device 1/BadCount");
        OpcValue Temperature = client.ReadNode("ns=2;s=Device 1/Temperature");
        OpcValue ProductionRate = client.ReadNode("ns=2;s=Device 1/ProductionRate");



        OpcValue DeviceErrors = client.ReadNode("ns=2;s=Device 1/DeviceError");
        uint errorValue = Convert.ToUInt32(DeviceErrors.Value);
        Errors errors = (Errors)errorValue;

    await device.SendMessages(ProductionStatus, WorkorderId, GoodCount, BadCount, Temperature);
    }


[Flags]
public enum Errors
{
    a = 0,
    b = 1,
    c = 2,
    d = 4,
    e = 8
};






