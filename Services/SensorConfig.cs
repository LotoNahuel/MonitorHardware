using System;
using System.IO;
using System.Text.Json;
using exportHardwareSensors;
using exportValueHardwareSensors;

namespace Program
{
    class ProgramX2
    {
        static void MainWindow()
        {
            hardwareSensors hardware = new hardwareSensors();
            valueSensors values = new valueSensors();
            // Dictionary<string, List<(string nameHardware, string nameSensor, string typeSensor)>> data = hardware.GetSensors();
            List<(string typeHardware, string nameHardware, string nameSensor, string typeSensor)> data = hardware.GetSensors();

            foreach (var sensor in data)
            {
                if (sensor.typeSensor == "Temperature")
                {
                    // Console.Clear();
                    string valor = values.GetValueSensors(sensor.nameSensor);
                    // Console.WriteLine($"\t{sensor.nameHardware} \n\t\t{sensor.nameSensor} - {valor} \t{sensor.typeSensor}");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine($"\t{sensor.nameHardware}");
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.Write($"\t\t{sensor.nameSensor}");
                    while (true)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write($"\r\t\t{valor}");
                    }
                }
            }
        }
    }
}

namespace saveJson
{
    public class ListConfig
    {
        public string TypeHardware {get; set;} = "";
        public string NameHardware {get; set;} = "";
        public string NameSensor {get; set;} = "";
        public string TypeSensor {get; set;} = "";
    }
    
    public static class configurationMonitor
    {
        public static async Task SaveConfiguration(
            IEnumerable<ListConfig> sensorConfigurations,
            string? filePath = null)
        {
            ArgumentNullException.ThrowIfNull(sensorConfigurations);

            string destination = filePath ?? Path.Combine(
                AppContext.BaseDirectory,
                "sensor-config.json");

            string jsonString = JsonSerializer.Serialize(
                sensorConfigurations,
                new JsonSerializerOptions { WriteIndented = true });

            await File.WriteAllTextAsync(destination, jsonString);
        }
    }
}

// namespace ListSensorConfig
// {
//     class ListConfig
//     {
//         public string NameHardware { get; set; } = "";
//         public string NameSensor { get; set; } = "";
//         public string TypeSensor { get; set; } = "";
//     }
// }

// codex resume 01a0fee8-c422-7cf2-b9fc-c83b5d20accc