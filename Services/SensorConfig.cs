using System;
using System.IO;
using System.Text.Json;
using exportHardwareSensors;
using exportValueHardwareSensors;

namespace saveJson
{
    public class ListConfig
    {
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