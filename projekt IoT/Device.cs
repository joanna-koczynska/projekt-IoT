
using Opc.UaFx;
using Opc.UaFx.Client;
using Microsoft.Azure.Devices.Client;
using System.Text;
using Newtonsoft.Json;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Shared;
using System.Data;
using System.Net.Mime;



using (var client = new OpcClient("opc.tcp://localhost:4840/"))
{
    client.Connect();


    OpcValue WorkorderId = client.ReadNode("ns=2;s=Device 1/WorkorderId");
    OpcValue ProductionStatus = client.ReadNode("ns=2;s=Device 1/ProductionStatus");
    OpcValue GoodCount = client.ReadNode("ns=2;s=Device 1/GoodCount");
    OpcValue BadCount = client.ReadNode("ns=2;s=Device 1/BadCount");
    OpcValue Temperature = client.ReadNode("ns=2;s=Device 1/Temperature");
    OpcValue ProductionRate = client.ReadNode("ns=2;s=Device 1/ProductionRate");
    

    if (ProductionStatus == 0)
        Console.WriteLine("Production Status: Stopped");

    else
        Console.WriteLine("Production Status: Running");
    Console.WriteLine($"Workorder ID: {WorkorderId}");
    Console.WriteLine($"Good Count: {GoodCount}");
    Console.WriteLine($"Bad Count: {BadCount}");
    Console.WriteLine($"Temperature: {Temperature}");
   
   OpcValue DeviceErrors = client.ReadNode("ns=2;s=Device 1/DeviceError");
        uint errorValue = Convert.ToUInt32(DeviceErrors.Value);
    Errors errors = (Errors)errorValue;

    if ((errors & Errors.a) == Errors.a)
    {
        Console.WriteLine("None");
    }

    if ((errors & Errors.b) == Errors.b)
    {
        Console.WriteLine("Emergency Stop");
    }

    if ((errors & Errors.c) == Errors.c)
    {
        Console.WriteLine("Power Failure");
    }

    if ((errors & Errors.d) == Errors.d)
    {
        Console.WriteLine("Sensor Failure");
    }

    if ((errors & Errors.e) == Errors.e)
    {
        Console.WriteLine("Unknown");
    }

//////////message
   
        string ConnectionString = "HostName=IoT-centrum.azure-devices.net;DeviceId=Device1;SharedAccessKey=kFvAwD1b2RVkGhoPbH5ESa4a/Htxfn2PTnYPz+a85Ns=";
    
    // Connect to the IoT Hub
        DeviceClient deviceClient = DeviceClient.CreateFromConnectionString(ConnectionString, TransportType.Mqtt);
    var MessageData = new
    {
        PStatus = ProductionStatus,
            WID = WorkorderId,
            GCount = GoodCount,
            BCount = BadCount,
            temperature = Temperature,
            DeErrors= errorValue
        };
        var messageJson = JsonConvert.SerializeObject(MessageData);
    Message message = new Message(Encoding.UTF8.GetBytes(messageJson));
    message.ContentType = MediaTypeNames.Application.Json;
    message.ContentEncoding = "utf-8";
    await deviceClient.SendEventAsync(message);



        // Close the connection
        await deviceClient.CloseAsync();
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

