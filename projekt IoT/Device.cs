
using Opc.UaFx;
using Opc.UaFx.Client;
using Microsoft.Azure.Devices.Client;


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
}

// TEST


[Flags]
public enum Errors
{
    a = 0,
    b = 1,
    c = 2,
    d = 4,
    e = 8
};
