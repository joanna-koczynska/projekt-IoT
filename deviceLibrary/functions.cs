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

        public async Task SendMessages(OpcValue ProductionStatus, OpcValue WorkorderId, OpcValue GoodCount, OpcValue BadCount, OpcValue Temperature)
        {
            var data = new
            {
                ProductionStatus = ProductionStatus,
                WorkorderId = WorkorderId,
                GoodCount = GoodCount,
                BadCount = BadCount,
                Temperature = Temperature,
            };

            var dataString = JsonConvert.SerializeObject(data);

            Message eventMessage = new Message(Encoding.UTF8.GetBytes(dataString));
            eventMessage.ContentType = MediaTypeNames.Application.Json;
            eventMessage.ContentEncoding = "utf-8";
            await client.SendEventAsync(eventMessage);
        }



        #endregion Sending Messages


        #region Device Twin

        public async Task UpdateTwinAsync()
        {
            var twin = await client.GetTwinAsync();

            Console.WriteLine($"\nInitial twin value received: \n{JsonConvert.SerializeObject(twin, Formatting.Indented)}");
            Console.WriteLine();

            var reportedProperties = new TwinCollection();
            reportedProperties["DateTimeLastAppLaunch"] = DateTime.Now;

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

    }

}

