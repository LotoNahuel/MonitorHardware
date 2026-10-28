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

namespace OverlayMonitorHardware
{
    public class OverlayDisplaySensorItem : INotifyPropertyChanged
    {
        // public string NameSensor { get; set; } = "";
        // public string ValueSensor { get; set; } = "";
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

    public partial class WidgetWindow : Window
    {
        valueSensors values = new valueSensors();
        public ObservableCollection<OverlayDisplaySensorItem>
            SensorValues { get; } = new();

        public class SensorJson
        {
            public string NameHardware { get; set; }
            public string NameSensor { get; set; }
            public string TypeSensor { get; set; }
        }

        // public async Task<List<SensorJson>> Sensor()
        // {
        //     string filePath = Path.Combine(AppContext.BaseDirectory,"sensor-config.json");

        //     // if (File.Exists(filePath))
        //     // {
        //         string jsonString = File.ReadAllText(filePath);
        //         List<SensorJson> Sensors = JsonSerializer.Deserialize<List<SensorJson>>(jsonString);

        //         if (Sensors != null)
        //         {
        //             foreach( var sensor in Sensors)
        //             {
                        
        //             }
        //         }
        //         else
        //         {
        //             return null;
        //         }
        //     // }
        // }
        // public string RefreshValues()
        // {
        //     // List<OverlayDisplaySensorItem> name = new List<OverlayDisplaySensorItem>();
        //     valueSensors values = new valueSensors();
        //     string filePath = Path.Combine(AppContext.BaseDirectory,"sensor-config.json");

        //     string jsonString = File.ReadAllText(filePath);
        //     List<SensorJson> Sensors = JsonSerializer.Deserialize<List<SensorJson>>(jsonString) ?? new List<SensorJson>();
        //     string valueSensor = "";
        //     foreach (var sensor in Sensors)
        //     {
        //         valueSensor = values.GetValueSensors(sensor.NameSensor);
        //         return valueSensor;
        //         // SensorValues.Add(new OverlayDisplaySensorItem
        //         // {
        //         //     ValueSensor = $"{values.GetValueSensors(sensor.NameSensor)?.ToString() ?? "N/A"}"
        //         // });
        //     }
        //     return "Vacio";
        // }
        // private async void WidgetWindow_Loaded(object sender, RoutedEventArgs e)
        // {
        //     await Task.Run(() => RefreshValues());
        // }
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
                case "SmallData":
                    return "GB";
                default:
                    return "";
            }
            // return type;
        }
        private void ActualizarSensores(bool bol, string typeSensor)
        {
            if (bol != false || typeSensor != "")
            {
                string type = "";
                foreach (var sensor in SensorValues)
                {
                    type = TSensor(typeSensor);
                    string valor = $"{values.GetValueSensors(sensor.NameSensor)} {type}";

                    sensor.ValueSensor = valor;
                }
            }
        }
        private readonly DispatcherTimer timer = new();
        public WidgetWindow(bool booleano)
        {
            if (booleano != false)
            {
                InitializeComponent();
                DataContext = this;

                List<OverlayDisplaySensorItem> name = new List<OverlayDisplaySensorItem>();
                string filePath = Path.Combine(AppContext.BaseDirectory,"sensor-config.json");
                string jsonString = File.ReadAllText(filePath);
                List<SensorJson> Sensors = JsonSerializer.Deserialize<List<SensorJson>>(jsonString) ?? new List<SensorJson>();
                
                string type = "";
                string typeSensor = "";
                if (Sensors != null)
                {
                    foreach (var sensor in Sensors)
                    {
                        type = TSensor(sensor.TypeSensor);
                        typeSensor = $"{sensor.TypeSensor}";
                        string valueSensor = values.GetValueSensors(sensor.NameSensor);
                        SensorValues.Add(new OverlayDisplaySensorItem
                        {
                            NameSensor = $"{sensor.NameSensor}",
                            ValueSensor = $"{valueSensor} {type}"
                        });
                        // Loaded += WidgetWindow_Loaded;
                    }
                }
                timer.Interval = TimeSpan.FromSeconds(0.15);
                timer.Tick += (sender, e) => ActualizarSensores(true, typeSensor);
                timer.Start();
            }
            else
            {
                timer.Interval = TimeSpan.FromHours(1);
                timer.Tick += (sender, e) => ActualizarSensores(false, "");
                timer.Stop();
            }
        }
    }
}