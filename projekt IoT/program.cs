using deviceLib;
using Microsoft.Azure.Devices.Client;
using Opc.UaFx.Client;
using Opc.UaFx;
using Microsoft.Azure.Amqp.Framing;
using Newtonsoft.Json;
using System.Text;
using System.Reflection.Emit;
using Org.BouncyCastle.Bcpg;
using System.CodeDom.Compiler;
using System.Net.Mime;

dynamic count = 0;
string ConnectionString = "HostName=IoT-centrum.azure-devices.net;DeviceId=Device1;SharedAccessKey=kFvAwD1b2RVkGhoPbH5ESa4a/Htxfn2PTnYPz+a85Ns=";
using var deviceClient = DeviceClient.CreateFromConnectionString(ConnectionString, TransportType.Mqtt);
await deviceClient.OpenAsync();
    var device = new functions(deviceClient);

using (var client = new OpcClient("opc.tcp://localhost:4840/"))
    {
        client.Connect();

    #region CountDevices
    static int CountDevices(OpcNodeInfo node)
    {
        int count = 0;
        int level = 0;
        Stack<OpcNodeInfo> nodesToProcess = new Stack<OpcNodeInfo>();
        nodesToProcess.Push(node);

        while (nodesToProcess.Count > 0)
        {
            OpcNodeInfo currentNode = nodesToProcess.Pop();
            string nazwa = currentNode.Attribute(OpcAttribute.DisplayName).Value.ToString();

            if (nazwa.StartsWith("Device "))
            {
                count++;
            }

            foreach (var childNode in currentNode.Children())
            {
                nodesToProcess.Push(childNode);
            }
        }

        return count;
    }
    #endregion

    try
    {

        var node = client.BrowseNode(OpcObjectTypes.ObjectsFolder);
        int c = CountDevices(node);
        Console.WriteLine(c);
        /*
                for (int i = 1; i <= c; i++)
                {

                    //OpcValue ProductionStatus = client.ReadNode($"ns=2;s=Device {i}/ProductionStatus");
 }  */


         Errors last_error = (Errors)16;
        OpcValue ProductionStatus = client.ReadNode($"ns=2;s=Device 1/ProductionStatus");

        while ((int)ProductionStatus.Value == 1 || (int)ProductionStatus.Value == 0)
            {
                OpcValue WorkorderId = client.ReadNode("ns=2;s=Device 1/WorkorderId");
                ProductionStatus = client.ReadNode("ns=2;s=Device 1/ProductionStatus");
                OpcValue GoodCount = client.ReadNode("ns=2;s=Device 1/GoodCount");
                OpcValue BadCount = client.ReadNode("ns=2;s=Device 1/BadCount");
                OpcValue Temperature = client.ReadNode("ns=2;s=Device 1/Temperature");

                OpcValue ProductionRate = client.ReadNode("ns=2;s=Device 1/ProductionRate");
                OpcValue DeviceErrors = client.ReadNode("ns=2;s=Device 1/DeviceError");
                uint errorValue = Convert.ToUInt32(DeviceErrors.Value);
                Errors errors = (Errors)errorValue;

                string PSResult = ((int)ProductionStatus.Value == 1) ? "Running" : "Stopped";

                #region telemetry
                await device.SendMessages(PSResult, WorkorderId, GoodCount, BadCount, Temperature);
            #endregion

            #region ERRORS->messages and device twin


            if (last_error != errors)
                {
                if ((errors & Errors.a) == Errors.a)
                {
                    await device.ReportedDeviceTwin("DeviceError1", "None");
                    last_error = errors;
                }
                else
                {

                    if ((errors & Errors.b) == Errors.b)
                    {
                        // var eventData = new Message(Encoding.UTF8.GetBytes("Emergency Stop"));
                        // await deviceClient.SendEventAsync(eventData);
                        await device.ReportedDeviceTwin("DeviceError2", "Emergency Stop");
                        last_error = errors;

                        var eventData = new
                        {
                            e = "Emergency Stop"
                        };

                        var dataString = JsonConvert.SerializeObject(eventData);

                        Message eventMessage = new Message(Encoding.UTF8.GetBytes(dataString));
                        eventMessage.ContentType = MediaTypeNames.Application.Json;
                        eventMessage.ContentEncoding = "utf-8";
                        await deviceClient.SendEventAsync(eventMessage);
                    }

                    if ((errors & Errors.c) == Errors.c)
                    {
                        var eventData = new Message(Encoding.UTF8.GetBytes("Power Failure"));
                        await deviceClient.SendEventAsync(eventData);
                        await device.ReportedDeviceTwin("DeviceError3", "Power Failure");
                        last_error = errors;
                    }

                    if ((errors & Errors.d) == Errors.d)
                    {
                        var eventData = new Message(Encoding.UTF8.GetBytes("Sensor Failure"));
                        await deviceClient.SendEventAsync(eventData);
                        await device.ReportedDeviceTwin("DeviceError4", "Sensor Failure");
                        last_error = errors;
                    }

                    if ((errors & Errors.e) == Errors.e)
                    {
                        var eventData = new Message(Encoding.UTF8.GetBytes("Unknown"));
                        await deviceClient.SendEventAsync(eventData);
                        await device.ReportedDeviceTwin("DeviceError5", "Unknown");
                        last_error = errors;
                    }
                }
                }
                #endregion


                #region Reported ProductionRate Device Twin



                await device.ReportedDeviceTwin("ProductionRate", ProductionRate.ToString() + "%");
                #endregion


            }

        }
    
    catch (Exception ex)
    {
        Console.WriteLine("Error conect with device");
    }
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






