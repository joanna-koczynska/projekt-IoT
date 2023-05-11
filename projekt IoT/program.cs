using deviceLib;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Amqp.Framing;
using Opc.UaFx;
using Opc.UaFx.Client;


//"HostName=IoT-centrum.azure-devices.net;DeviceId=Device1;SharedAccessKey=kFvAwD1b2RVkGhoPbH5ESa4a/Htxfn2PTnYPz+a85Ns=";
System.Console.WriteLine("Type device Connection String (confirm with enter):");
string deviceConnectionString = System.Console.ReadLine() ?? string.Empty;
using var deviceClient = DeviceClient.CreateFromConnectionString(deviceConnectionString, TransportType.Mqtt);
await deviceClient.OpenAsync();
Console.WriteLine("IoT Device Connection success!");
Console.WriteLine();


//"opc.tcp://localhost:4840/"
System.Console.WriteLine("Type OPC.UA Connection (confirm with enter):");
string connOPC = System.Console.ReadLine() ?? string.Empty;
var OPCclient = new OpcClient(connOPC);
OPCclient.Connect();
Console.WriteLine("OPC UA Connection success!");

var device = new functions(deviceClient, OPCclient);
OpcValue ProductionStatus = OPCclient.ReadNode($"ns=2;s=Device 1/ProductionStatus");

while ((int)ProductionStatus.Value == 0 || (int)ProductionStatus.Value == 1)
    {
        ProductionStatus = OPCclient.ReadNode($"ns=2;s=Device 1/ProductionStatus");
        string PSResult = ((int)ProductionStatus.Value == 1) ? "Running" : "Stopped";
 
    string WorkorderIdS = "";
    if (PSResult == "Stopped")
    { WorkorderIdS = ""; }
    else
    {
        OpcValue WorkorderId = OPCclient.ReadNode("ns=2;s=Device 1/WorkorderId");
        WorkorderIdS = (string)WorkorderId.Value; 
    }

    OpcValue GoodCount = OPCclient.ReadNode("ns=2;s=Device 1/GoodCount");
        OpcValue BadCount = OPCclient.ReadNode("ns=2;s=Device 1/BadCount");
        OpcValue Temperature = OPCclient.ReadNode("ns=2;s=Device 1/Temperature");

        var data = new
        {
            ProductionStatus = PSResult,
            WorkorderId = WorkorderIdS,
            GoodCount = GoodCount.Value,
            BadCount = BadCount.Value,
            Temperature = Temperature.Value,
        };

        OpcValue ProductionRate = OPCclient.ReadNode("ns=2;s=Device 1/ProductionRate");
        OpcValue DeviceErrors = OPCclient.ReadNode("ns=2;s=Device 1/DeviceError");

    //functions
    await device.InitializeHandlers();
    await device.SendTelemetry(data);
    await device.TwinAsync(DeviceErrors, ProductionRate);

}


  

OPCclient.Disconnect();
Console.WriteLine("Finished! Press Enter to close...");
Console.ReadLine();




/*enum Errors
{
    None = 0,
    EmergencyStop = 1,
    PowerFailure = 2,
    SensorFailue = 4,
    Unknown = 8
}
*/



  






