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
using System.Windows.Threading;
using LibreHardwareMonitor.Hardware;
using LibreHardwareMonitor.Interop.PowerMonitor;
using System.Runtime.CompilerServices;
using System.Configuration;

namespace OverlayMonitorHardware
{
    public class OverlayDisplaySensorItem : INotifyPropertyChanged
    {
        public string NameSensor { get; set; } = "";

        private string _valueSensor = "";

        public string ValueSensor
        {
            get => _valueSensor;
            set
            {
                if (_valueSensor == value)
                    return;

                _valueSensor = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(
            [CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(
                this,
                new PropertyChangedEventArgs(propertyName));
        }
    }

    public class OverlayDisplayHardwareGroup
    {
        public string NameHardware { get; set; } = "";
        public ObservableCollection<OverlayDisplaySensorItem> Sensors { get; } = new();
    }

    public partial class WidgetWindow : Window
    {
        valueSensors values = new valueSensors();
        public ObservableCollection<OverlayDisplayHardwareGroup>
            HardwareGroups { get; } = new();

        private readonly DispatcherTimer timer = new();
        private readonly List<string> sensorNames = new();

        public class SensorJson
        {
            public string NameHardware { get; set; }
            public string NameSensor { get; set; }
            public string TypeSensor { get; set; }
        }
        private string TSensor(string typeSensor)
        {
            switch (typeSensor)
            {
                case "Load":
                    return "%";
                case "Temperature":
                    return "°C";
                case "Power":
                    return "W";
                case "Voltage":
                    return "V";
                case "Clock":
                    return "MHz";
                case "Data":
                    return "GB";
                case "SmallData":
                    return "MB";
                case "Timing":
                    return "ms";
                case "Level":
                    return "%";
                case "Factor":
                    return "";
                case "Throughput":
                    return "MB/s";
                case "Fan":
                    return "RPM";
                default:
                    return "";
            }
        }
        private void ActualizarSensores()
        {
            var sensorGet = values.GetValueSensors(sensorNames);
            foreach (var group in HardwareGroups)
            {
                foreach (var sensor in group.Sensors)
                {
                    var current = sensorGet.FirstOrDefault(value => value.name == sensor.NameSensor);
                    if (current.name != null && current.value != "N/A")
                    {
                        sensor.ValueSensor = $"{current.value} {TSensor(current.type)}".Trim();
                    }
                    else
                    {
                        sensor.ValueSensor = "N/A";
                    }
                }
            }
        }

        public void StopTimer()
        {
            timer.Stop();
        }

        public WidgetWindow(bool booleano)
        {
            if (booleano != false)
            {
                InitializeComponent();
                DataContext = this;

                string filePath = Path.Combine(AppContext.BaseDirectory, "sensor-config.json");
                string jsonString = File.ReadAllText(filePath);
                List<SensorJson> Sensors = JsonSerializer.Deserialize<List<SensorJson>>(jsonString) ?? new List<SensorJson>();

                foreach (var sensor in Sensors)
                {
                    var group = HardwareGroups.FirstOrDefault(item => item.NameHardware == sensor.NameHardware);
                    
                    if (group == null)
                    {
                        group = new OverlayDisplayHardwareGroup { NameHardware = sensor.NameHardware };
                        HardwareGroups.Add(group);
                    }

                    sensorNames.Add(sensor.NameSensor);

                    group.Sensors.Add(new OverlayDisplaySensorItem
                    {
                        NameSensor = sensor.NameSensor
                    });
                }

                ActualizarSensores();
                timer.Interval = TimeSpan.FromMilliseconds(250);
                timer.Tick += (sender, e) => ActualizarSensores();
                timer.Start();
            }
        }
    }
}

// with laptop
// codex resume 01a19079-3d7c-7df3-a5b6-79a7008d8dc3