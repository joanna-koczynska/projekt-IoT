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
            string PSResult = (ProductionStatus == 1) ? "Running" : "Stopped";

            var data = new
            {
                ProductionStatus = PSResult,
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

        public async Task ReportedDeviceTwin(string propertyName, string propertyValue)
        {
            var twin = await client.GetTwinAsync();
            var reportedProperties = new TwinCollection();
            reportedProperties[propertyName] = propertyValue;

            await client.UpdateReportedPropertiesAsync(reportedProperties);
        }

       /* private async Task OnDesiredPropertyChanged(TwinCollection desiredProperties, object userContext)
        {
            Console.WriteLine($"\tDesired property change:\n\t{JsonConvert.SerializeObject(desiredProperties)}");
            Console.WriteLine("\tSending current time as reported property");
            TwinCollection reportedProperties = new TwinCollection();
            reportedProperties["DateTimeLastDesiredPropertyChangeReceived"] = DateTime.Now;

            await client.UpdateReportedPropertiesAsync(reportedProperties).ConfigureAwait(false);
        }*/



        #endregion Device Twin

    }
    /*    #region Direct Method
          {
                            System.Console.WriteLine("\nType your device ID (confirm with enter):");
                            string deviceId = System.Console.ReadLine() ?? string.Empty;
                            try
                            {
                                var result = await manager.ExecuteDeviceMethod("SendMessages", deviceId);
        System.Console.WriteLine($"Method executed with status {result}");
                            }
                            catch (DeviceNotFoundException)
                            {
        System.Console.WriteLine("Device not connected!");
    }
                        }

        #endregion*/

}

