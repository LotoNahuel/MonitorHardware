using System;
using System.IO;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Text.Json;
using saveJson;
using System.Security.Cryptography.X509Certificates;
using System.Collections.ObjectModel;
using exportHardwareSensors;
using exportValueHardwareSensors;
using LibreHardwareMonitor.Interop.PowerMonitor;

namespace OverlayMonitorHardware
{
    public class OverlayDisplaySensorItem
    {
        public string NameSensor { get; set; } = "";
        public string ValueSensor { get; set; } = "";
    }

    public partial class WidgetWindow : Window
    {
        public ObservableCollection<OverlayDisplaySensorItem>
            SensorValues { get; } = new();

        public class SensorJson
        {
            public string NameHardware { get; set; }
            public string NameSensor { get; set; }
            public string TypeSensor { get; set; }
        }

        public async Task<List<SensorJson>> Sensor()
        {
            string filePath = Path.Combine(AppContext.BaseDirectory,"sensor-config.json");

            // if (File.Exists(filePath))
            // {
                string jsonString = File.ReadAllText(filePath);
                List<SensorJson> Sensors = JsonSerializer.Deserialize<List<SensorJson>>(jsonString);

                if (Sensors != null)
                {
                    foreach( var sensor in Sensors)
                    {
                        
                    }
                }
                else
                {
                    return null;
                }
            // }
        }
        public string RefreshValues()
        {
            valueSensors values = new valueSensors();
            string filePath = Path.Combine(AppContext.BaseDirectory,"sensor-config.json");

            string jsonString = File.ReadAllText(filePath);
            List<SensorJson> Sensors = JsonSerializer.Deserialize<List<SensorJson>>(jsonString) ?? new List<SensorJson>();
            string valueSensor = "";
            foreach (var sensor in Sensors)
            {
                valueSensor = values.GetValueSensors(sensor.NameSensor);
            }
            return valueSensor;
        }
        // private async void WidgetWindow_Loaded(object sender, RoutedEventArgs e)
        // {
        //     await Task.Run(() => RefreshValues());
        // }

        public WidgetWindow()
        {
            InitializeComponent();
            DataContext = this;
            
            List<OverlayDisplaySensorItem> name = new List<OverlayDisplaySensorItem>();
            // string filePath = Path.Combine(AppContext.BaseDirectory,"sensor-config.json");

            // string jsonString = File.ReadAllText(filePath);
            // List<SensorJson> Sensors = JsonSerializer.Deserialize<List<SensorJson>>(jsonString) ?? new List<SensorJson>();
            
            if (List<SensorJson>Sensors() != null)
            {
                string type = "";
                foreach (var sensor in Sensors)
                {
                    switch (sensor.TypeSensor)
                    {
                        case "Load":
                            type = "%";
                            break;
                        case "Temperature":
                            type = "°C";
                            break;
                        case "Power":
                            type = "W";
                            break;
                        case "Voltage":
                            type = "V";
                            break;
                        case "Clock":
                            type = "MHz";
                            break;
                        case "SmallData":
                            type = "GB";
                            break;
                        default:
                            type = "";
                            break;
                    }

                    SensorValues.Add(new OverlayDisplaySensorItem
                    {
                        NameSensor = $"{sensor.NameSensor}",
                        ValueSensor = $"{RefreshValues()?.ToString() ?? "N/A"} {type}"
                    });
                }
            }            
        }
    }
}