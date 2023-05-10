using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Shared;
using Newtonsoft.Json;
using Opc.UaFx.Client;
using Opc.UaFx;
using System.Net.Mime;
using System.Text;
using System.Xml;
using Microsoft.VisualBasic;
using System.Threading.Tasks;
using Microsoft.Azure.Amqp.Framing;
using System.Net.Sockets;

namespace deviceLib
{
    public class functions
    {
        private DeviceClient client;

        private  OpcClient OPC;

        public functions(DeviceClient deviceClient, OpcClient OPC)
        {
            this.client = deviceClient;
            this.OPC = OPC;
        }

        #region D2C - Sending telemetry

        public async Task SendTelemetry(dynamic data)
        {
            var dataString = JsonConvert.SerializeObject(data);

            Message eventMessage = new Message(Encoding.UTF8.GetBytes(dataString));
            eventMessage.ContentType = MediaTypeNames.Application.Json;
            eventMessage.ContentEncoding = "utf-8";
            await client.SendEventAsync(eventMessage);
            if (true)
                await Task.Delay(10000);

        }
        #endregion

        #region D2C - event
        public async Task D2C_Event(dynamic data)
        {
            var dataString = JsonConvert.SerializeObject(data);

            Message eventMessage = new Message(Encoding.UTF8.GetBytes(dataString));
            eventMessage.ContentType = MediaTypeNames.Application.Json;
            eventMessage.ContentEncoding = "utf-8";
            await client.SendEventAsync(eventMessage);
        }

        #endregion


        #region Receiving Message
        private async Task OnC2dMessageReceivedAsync(Message receivedMessage, object _)
        {
            Console.WriteLine($"\t{DateTime.Now}> C2D Message Callback - message received with Id = {receivedMessage.MessageId}");
            PrintMessage(receivedMessage);
            await client.CompleteAsync(receivedMessage);
            Console.WriteLine($"\t {DateTime.Now}> Completed C2D message with Id = {receivedMessage.MessageId}");
        }
        private void PrintMessage(Message receivedMessage)
        {
            string messageData = Encoding.ASCII.GetString(receivedMessage.GetBytes());
            Console.WriteLine($"\t\tReceived message: {messageData}");

            int propCount = 0;
            foreach (var prop in receivedMessage.Properties)
            {
                Console.WriteLine($"\t\tProperty[{propCount++}]> Key={prop.Key} : Value = {prop.Value}");
            }
        }

        #endregion

        #region Device Twin

       
        public async Task<bool>UpdateTwinValueAsync(string name, dynamic value)
        {
            bool upp=false;
            try
            {
                var twin = await client.GetTwinAsync();
                /*  Console.WriteLine($"\n Initial twin value received: \n{JsonConvert.SerializeObject(twin, Formatting.Indented)}");
                  Console.WriteLine();*/

                var reportedProperties = new TwinCollection();
                reportedProperties[name] = value;
                await client.UpdateReportedPropertiesAsync(reportedProperties);
                upp = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\t{DateTime.Now.ToLocalTime()}> Twin update failed with exception: {ex.Message}");
                upp = false;
            }
            return upp;

        }
        public async Task TwinAsync(dynamic DeviceErrors, dynamic ProductionRate)
        {
            string errorResult = string.Empty;
            uint errorValue = Convert.ToUInt32(DeviceErrors.Value);
            Errors errors = (Errors)errorValue;

            if (errors != 0)
            {
                int c = 0;
                if ((errors & Errors.EmergencyStop) == Errors.EmergencyStop)
                {
                    errorResult += "Emergency stop";
                    c++;

                }
                if ((errors & Errors.PowerFailure) == Errors.PowerFailure)
                {
                    errorResult += "PowerFailure, ";
                    c++;
                }
                if ((errors & Errors.SensorFailue) == Errors.SensorFailue)
                {
                    errorResult += "SensorFailure, ";
                    c++;
                }
                if ((errors & Errors.Unknown) == Errors.Unknown)
                {
                    errorResult += "Unknown, ";
                    c++;
                }

                var data = new
                {
                    DeviceErrors = errorResult,
                    quantity = c
                };
               
                await UpdateTwinValueAsync("DeviceErrors", errorResult);
                if (await UpdateTwinValueAsync("DeviceErrors", errorResult)==true)
                {
                    await D2C_Event(data);
                }
                
                    
            }
            await UpdateTwinValueAsync("ProductionRate", ProductionRate+"%");

        }



        private async Task OnDesiredPropertyChanged(TwinCollection desiredProperties, object _)
        {
            Console.WriteLine($"\tDesired property change:\n\t{JsonConvert.SerializeObject(desiredProperties)}");
            Console.WriteLine("\tSending current time as reported property");
            TwinCollection reportedProperties = new TwinCollection();
            reportedProperties["DateTimeLastDesiredPropertyChangeReceived"] = DateTime.Now;

            await client.UpdateReportedPropertiesAsync(reportedProperties);
        }

        #endregion


        #region Direct Methods
       
        public async Task EmergencyStopDM()
        { 
            Console.WriteLine($"\tDevice shut down 1\n");
        OPC.CallMethod($"ns=2;s=Device 1", $"ns=2;s=Device 1/EmergencyStop");
        OPC.WriteNode($"ns=2;s=Device 1/ProductionRate", OpcAttribute.Value, 0);
        await Task.Delay(1000);
    }
    private async Task<MethodResponse> EmergencyStopHandler(MethodRequest methodRequest, object userContext)
    {
        Console.WriteLine($"\tMETHOD EXECUTED: {methodRequest.Name}");

        var payload = JsonConvert.DeserializeAnonymousType(methodRequest.DataAsJson, new { machineId = default(string) });

            await EmergencyStopDM();

            return new MethodResponse(0);
    }

    public async Task ResetErrorStatus()
    {
        OPC.CallMethod($"ns=2;s=Device 1", $"ns=2;s=Device 1/ResetErrorStatus");
        await Task.Delay(1000);
    }
    private async Task<MethodResponse> ResetErrorStatusHandler(MethodRequest methodRequest, object userContext)
    {
        Console.WriteLine($"\tMETHOD EXECUTED: {methodRequest.Name}");

        var payload = JsonConvert.DeserializeAnonymousType(methodRequest.DataAsJson, new { machineId = default(string) });

        await ResetErrorStatus();

        return new MethodResponse(0);
    }
        private static async Task<MethodResponse> DefaultServiceHandler(MethodRequest methodRequest, object userContext)
        {
            Console.WriteLine($"\tMETHOD EXECUTED: {methodRequest.Name}");

            await Task.Delay(1000);

            return new MethodResponse(0);

        }

        public async Task DecreaseDesiredProductionRate()
        {
            int productionRate = (int)OPC.ReadNode($"ns=2;s=Device 1/ProductionRate").Value;

            OPC.WriteNode($"ns=2;s=Device 1/ProductionRate", productionRate == 0 ? productionRate : productionRate - 10);
            OPC.Disconnect();
            await Task.Delay(1000);
        }
        private async Task<MethodResponse> DecreaseDesiredProductionRate(MethodRequest methodRequest, object userContext)
        {
            System.Console.WriteLine($"\tEXECUTED METHOD: {methodRequest.Name}");

            var payload = JsonConvert.DeserializeAnonymousType(methodRequest.DataAsJson, new { machineId = default(string) });

            await DecreaseDesiredProductionRate();

            return new MethodResponse(0);
        }

        #endregion

        public async Task InitializeHandlers()
    {
        await client.SetReceiveMessageHandlerAsync(OnC2dMessageReceivedAsync, client);

        await client.SetMethodHandlerAsync("EmergencyStop", EmergencyStopHandler, client);
        await client.SetMethodHandlerAsync("ResetErrorStatus", ResetErrorStatusHandler, client);
        await client.SetMethodDefaultHandlerAsync(DefaultServiceHandler, client);
        await client.SetMethodHandlerAsync("DecreaseDesiredProductionRate", DecreaseDesiredProductionRate, client);
      
        }

   }

}
    

enum Errors
{
    EmergencyStop = 1,
    PowerFailure = 2,
    SensorFailue = 4,
    Unknown = 8
}

