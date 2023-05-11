using Microsoft.Azure.Amqp.Framing;
using Microsoft.Azure.Devices.Common.Exceptions;
using System.Reflection.PortableExecutable;

namespace ServiceSdkDemo.Console
{
    internal static class FeatureSelector
    {
        public static void PrintMenu()
        {
            System.Console.WriteLine(@"
    1 - C2D
    2 - Direct Method - Emergency Stop
    3 - Direct Method - Reset Error Status
    4 - Device Twin 
    5 - Decrease Production Rate
    0 - Exit");
        }

        public static async Task Execute(int feature, Lib.IoTHubManager manager)
        {
            switch (feature)
            {
                case 1: // C2D
                    {
                        System.Console.WriteLine("\nType your message (confirm with enter):");
                        string messageText = System.Console.ReadLine() ?? string.Empty;

                        System.Console.WriteLine("Type device ID (confirm with enter):");
                        string deviceId = System.Console.ReadLine() ?? string.Empty;

                        await manager.SendMessage(messageText, deviceId);

                        System.Console.WriteLine("Message sent!");
                    }
                    break;
                case 2: // Direct Method - Emergency Stop
                    {
                        System.Console.WriteLine("\nType device ID (confirm with enter):");
                        string deviceId = System.Console.ReadLine() ?? string.Empty;
                        System.Console.WriteLine("\nType OPC device ID (confirm with enter):");
                        string OPC = System.Console.ReadLine() ?? string.Empty;
                        try
                        {
                            var result = await manager.ExecuteEmergencyStop("EmergencyStop", deviceId,OPC);
                            System.Console.WriteLine($"Method executed with status {result}");
                        }
                        catch (DeviceNotFoundException)
                        {
                            System.Console.WriteLine("Device not connected!");
                        }
                    }
                    break;

                case 3: // Direct Method -Reset Error Status
                    {
                        System.Console.WriteLine("\nType device ID (confirm with enter):");
                        string deviceId = System.Console.ReadLine() ?? string.Empty;
                        System.Console.WriteLine("\nType OPC device ID (confirm with enter):");
                        string OPC = System.Console.ReadLine() ?? string.Empty;
                        try
                        {
                            var result = await manager.ExecuteResetErrorStatus("ResetErrorStatus", deviceId, OPC);
                            System.Console.WriteLine($"Method executed with status {result}");
                        }
                        catch (DeviceNotFoundException)
                        {
                            System.Console.WriteLine("Device not connected!");
                        }
                    }
                    break;

                case 4://Device Twin
                    {
                        System.Console.WriteLine("\nType property name (confirm with enter):");
                        string propertyName = System.Console.ReadLine() ?? string.Empty;

                        System.Console.WriteLine("\nType device ID (confirm with enter):");
                        string deviceId = System.Console.ReadLine() ?? string.Empty;

                        var random = new Random();
                        await manager.UpdateDesiredTwin(deviceId, propertyName, random.Next());
                    }
                    break;

               

                case 5://Decrease Production Rate
                    {
                        System.Console.WriteLine("\nType device ID (confirm with enter):");
                        string deviceId = System.Console.ReadLine() ?? string.Empty;
                        System.Console.WriteLine("\nType OPC device ID (confirm with enter):");
                        string OPC = System.Console.ReadLine() ?? string.Empty;
                        try
                        {
                            var result = await manager.ExecuteDecreasePR("DecreasePR", deviceId, OPC);
                            System.Console.WriteLine($"Method executed with status {result}");
                        }
                        catch (DeviceNotFoundException)
                        {
                            System.Console.WriteLine("Device not connected!");
                        }
                    }
                    break;

                default:
                    break;
            }
        }

        internal static int ReadInput()
        {
            var keyPressed = System.Console.ReadKey();
            var isParsed = int.TryParse(keyPressed.KeyChar.ToString(), out int result);
            return isParsed ? result : -1;
        }
    }
}
