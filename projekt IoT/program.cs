using deviceLib;
using Microsoft.Azure.Devices.Client;
using Opc.UaFx.Client;
using Opc.UaFx;
using Microsoft.Azure.Amqp.Framing;
using Newtonsoft.Json;
using System.Text;

    string ConnectionString = "HostName=IoT-centrum.azure-devices.net;DeviceId=Device1;SharedAccessKey=kFvAwD1b2RVkGhoPbH5ESa4a/Htxfn2PTnYPz+a85Ns=";
using var deviceClient = DeviceClient.CreateFromConnectionString(ConnectionString, TransportType.Mqtt);
await deviceClient.OpenAsync();
    var device = new functions(deviceClient);

using (var client = new OpcClient("opc.tcp://localhost:4840/"))
    {
        client.Connect();
    try
    {

        OpcValue WorkorderId = client.ReadNode("ns=2;s=Device 1/WorkorderId");
        OpcValue ProductionStatus = client.ReadNode("ns=2;s=Device 1/ProductionStatus");
        OpcValue GoodCount = client.ReadNode("ns=2;s=Device 1/GoodCount");
        OpcValue BadCount = client.ReadNode("ns=2;s=Device 1/BadCount");
        OpcValue Temperature = client.ReadNode("ns=2;s=Device 1/Temperature");

        OpcValue ProductionRate = client.ReadNode("ns=2;s=Device 1/ProductionRate");
        OpcValue DeviceErrors = client.ReadNode("ns=2;s=Device 1/DeviceError");
        uint errorValue = Convert.ToUInt32(DeviceErrors.Value);
        Errors errors = (Errors)errorValue;


        #region telemetry
        await device.SendMessages(ProductionStatus, WorkorderId, GoodCount, BadCount, Temperature);
        #endregion


        #region event
        if ((errors & Errors.a) == Errors.a)
        {
            var eventData = new Message(Encoding.UTF8.GetBytes("None"));
            await deviceClient.SendEventAsync(eventData);
        }
        else
        {

            if ((errors & Errors.b) == Errors.b)
            {
                var eventData = new Message(Encoding.UTF8.GetBytes("Emergency Stop"));
                await deviceClient.SendEventAsync(eventData);
            }

            if ((errors & Errors.c) == Errors.c)
            {
                var eventData = new Message(Encoding.UTF8.GetBytes("Power Failure"));
                await deviceClient.SendEventAsync(eventData);
            }

            if ((errors & Errors.d) == Errors.d)
            {
                var eventData = new Message(Encoding.UTF8.GetBytes("Sensor Failure"));
                await deviceClient.SendEventAsync(eventData);
            }

            if ((errors & Errors.e) == Errors.e)
            {
                var eventData = new Message(Encoding.UTF8.GetBytes("Unknown"));
                await deviceClient.SendEventAsync(eventData);
            }
        }
        #endregion


        #region Reported Device Twin

        if ((errors & Errors.a) == Errors.a)
        {

            await device.ReportedDeviceTwin("DeviceError1", "None");
        }
        else
        {

            if ((errors & Errors.b) == Errors.b)
            {
                await device.ReportedDeviceTwin("DeviceError2", "Emergency Stop");
            }

            if ((errors & Errors.c) == Errors.c)
            { 
                await device.ReportedDeviceTwin("DeviceError3", "Power Failure");
            }

            if ((errors & Errors.d) == Errors.d)
            {
                await device.ReportedDeviceTwin("DeviceError4", "Sensor Failure");
            }

            if ((errors & Errors.e) == Errors.e)
            {
                await device.ReportedDeviceTwin("DeviceError5", "Unknown");
            }

        }

       await device.ReportedDeviceTwin("ProductionRate", ProductionRate.ToString()+"%");
        #endregion


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






