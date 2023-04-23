/*using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Common.Exceptions;


namespace ProjectFunction
{
    internal static class azure
    {
        public static async Task Execute(int feature, ServiceSdkDemo.Lib.IoTHubManager manager)
        {
            switch (feature)
            {
                case 1:
                    {
                        System.Console.WriteLine("\nType your message (confirm with enter):");
                        string messageText = System.Console.ReadLine() ?? string.Empty;

                        System.Console.WriteLine("\nType your device Id (confirm with enter):");
                        string deviceId = System.Console.ReadLine() ?? string.Empty;

                        await manager.SendMessage(messageText, deviceId);

                    }
                    break;
                case 2:
                    {
                        System.Console.WriteLine("\nType your device Id (confirm with enter):");
                        string deviceId = System.Console.ReadLine() ?? string.Empty;
                        try
                        {
                            var result = await manager.ExecuteDeviceMethod("SendMessages", deviceId);
                            System.Console.WriteLine($"Method executed with statis {result}");
                        }
                        catch (DeviceNotFoundException)
                        {
                            System.Console.WriteLine("Device not connected!");
                        }
                    }
                    break;

                case 3:
                    {
                        System.Console.WriteLine("\nType your device Id (confirm with enter):");
                        string deviceId = System.Console.ReadLine() ?? string.Empty;

                        System.Console.WriteLine("\nType property name (confirm with enter):");
                        string propertyname = System.Console.ReadLine() ?? string.Empty;

                        var random = new Random();
                        await manager.UpdateDesiredTwin(deviceId, propertyname, random.Next());
                    }
                    break;
                default:
                    break;
            }
        }
    }
}*/