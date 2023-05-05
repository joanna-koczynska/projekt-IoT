using Newtonsoft.Json;
using Opc.UaFx;
using Opc.UaFx.Client;
using System.Net.Mime;
using System.ServiceModel.Channels;
using System.Text;


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

   /* if ((errors & Errors.a) == Errors.a)
    {
        Console.WriteLine("None");
    }*/


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
        
      
    
