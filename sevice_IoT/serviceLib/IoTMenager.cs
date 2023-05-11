using Microsoft.Azure.Devices;
using Newtonsoft.Json;
using System.Text;

namespace ServiceSdkDemo.Lib
{
    public class IoTHubManager
    {
        private readonly ServiceClient client;
        private readonly RegistryManager registry;

        public IoTHubManager(ServiceClient client, RegistryManager registry)
        {
            this.client = client;
            this.registry = registry;
        }

        public async Task SendMessage(string messageText, string deviceId)
        {
            var messageBody = new { text = messageText };
            var message = new Message(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(messageBody)));
            message.MessageId = Guid.NewGuid().ToString();
            await client.SendAsync(deviceId, message);
        }

        public async Task<int> ExecuteResetErrorStatus(string methodName, string deviceId, string OPCId= default(string))
        {
            methodName = "ResetErrorStatus";
            var method = new CloudToDeviceMethod(methodName);

            var methodBody = new { OPC=OPCId };
            method.SetPayloadJson(JsonConvert.SerializeObject(methodBody));

            var result = await client.InvokeDeviceMethodAsync(deviceId, method);
            return result.Status;
        }


        public async Task<int> ExecuteEmergencyStop(string methodName, string deviceId, string OPCId = default(string))
        {
            methodName = "EmergencyStop";
            var method = new CloudToDeviceMethod(methodName);

            var methodBody = new { OPC = OPCId };
            method.SetPayloadJson(JsonConvert.SerializeObject(methodBody));

            var result = await client.InvokeDeviceMethodAsync(deviceId, method);
            return result.Status;
        }

        public async Task<int> ExecuteDecreasePR(string methodName, string deviceId, string OPCId = default(string))
        {
            methodName = "DecreasePR";
            var method = new CloudToDeviceMethod(methodName);

            var methodBody = new { OPC = OPCId };
            method.SetPayloadJson(JsonConvert.SerializeObject(methodBody));

            var result = await client.InvokeDeviceMethodAsync(deviceId, method);
            return result.Status;
        }




        public async Task UpdateDesiredTwin(string deviceId, string propertyName, dynamic propertyValue)
        {
            var twin = await registry.GetTwinAsync(deviceId);
            twin.Properties.Desired[propertyName] = propertyValue;
            await registry.UpdateTwinAsync(twin.DeviceId, twin, twin.ETag);
        }
    }
}
