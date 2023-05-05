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




[Flags]
public enum Errors
{
    a = 0,
    b = 1,
    c = 2,
    d = 4,
    e = 8
};

namespace deviceLib
{
    public class device
    {
        private readonly DeviceClient client;

        public device(DeviceClient deviceClient)
        {
            this.client = deviceClient;
        }

        #region Sending Messages

        public async Task SendMessages(OpcValue ProductionStatus, OpcValue WorkorderId, OpcValue GoodCount, OpcValue BadCount, OpcValue Temperature, uint ErrorValue)
        {
                var data = new
                {
                    PStatus = ProductionStatus,
                    WID = WorkorderId,
                    GCount = GoodCount,
                    BCount = BadCount,
                    temperature = Temperature,
                    DeErrors = ErrorValue
                };

                var dataString = JsonConvert.SerializeObject(data);

                Message eventMessage = new Message(Encoding.UTF8.GetBytes(dataString));
                eventMessage.ContentType = MediaTypeNames.Application.Json;
                eventMessage.ContentEncoding = "utf-8";
                await client.SendEventAsync(eventMessage);
            }
         
        }

    #endregion Sending Messages

    public void ReadDeviceData()
    {
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

            SendMessages(ProductionStatus, WorkorderId, GoodCount, BadCount, Temperature, errorValue);

        }
    }

    public async Task SendDeviceData()
    {
        // Wywołanie metody ReadDeviceData() do odczytu danych z urządzenia
        ReadDeviceData();

       
    }

}

