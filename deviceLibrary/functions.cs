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




namespace deviceLib
{
    public class functions
    {
        private readonly DeviceClient client;

        public functions(DeviceClient deviceClient)
        {
            this.client = deviceClient;
        }

        #region Sending Messages

        public async Task SendMessages(string ProductionStatus, OpcValue WorkorderId, OpcValue GoodCount, OpcValue BadCount, OpcValue Temperature)
        {
           
                var data = new
                {
                    ProductionStatus = ProductionStatus,
                    WorkorderId = WorkorderId.Value,
                    GoodCount = GoodCount.Value,
                    BadCount = BadCount.Value,
                    Temperature = Temperature.Value,
                };

                var dataString = JsonConvert.SerializeObject(data);

                Message eventMessage = new Message(Encoding.UTF8.GetBytes(dataString));
                eventMessage.ContentType = MediaTypeNames.Application.Json;
                eventMessage.ContentEncoding = "utf-8";
                await client.SendEventAsync(eventMessage);
                if(true)
                await Task.Delay(10000);
            
        }

        #endregion Sending Messages


        #region Device Twin

        public async Task ReportedDeviceTwin(string propertyName, string propertyValue)
        {
            var twin = await client.GetTwinAsync();
            var reportedProperties = new TwinCollection();
            reportedProperties[propertyName] = propertyValue;

            await client.UpdateReportedPropertiesAsync(reportedProperties);
        }

        private async Task OnDesiredPropertyChanged(TwinCollection desiredProperties, object userContext)
        {
            Console.WriteLine($"\tDesired property change:\n\t{JsonConvert.SerializeObject(desiredProperties)}");
            Console.WriteLine("\tSending current time as reported property");
            TwinCollection reportedProperties = new TwinCollection();
            reportedProperties["DateTimeLastDesiredPropertyChangeReceived"] = DateTime.Now;

            await client.UpdateReportedPropertiesAsync(reportedProperties).ConfigureAwait(false);
        }
        #endregion Device Twin

        
/*
        #region Direct Methods
        private async Task<MethodResponse> SendMessagesHandler(MethodRequest methodRequest, object userContext)
        {
            Console.WriteLine($"\tMETHOD EXECUTED : {methodRequest.Name}");
            var payload = JsonConvert.DeserializeAnonymousType(methodRequest.DataAsJson, new { nrOfMessages = default(int), delay = default(int) });
           
            return new MethodResponse(0);
        }
        private async Task<MethodResponse> DefaultServiceHandler(MethodRequest methodRequest, object userContext)
        {
            Console.WriteLine($"\tMETHOS EXECUTED : {methodRequest.Name}");

            await Task.Delay(1000);

            return new MethodResponse(0);
        }

        #endregion*/



       /* public async Task InitializeHandlers()
        {
            await client.SetMethodDefaultHandlerAsync(DefaultServiceHandler, client);

            await client.SetMethodHandlerAsync("SendMessages", SendMessagesHandler, client);

            await client.SetDesiredPropertyUpdateCallbackAsync(OnDesiredPropertyChanged, client);

        }*/


    }


}

