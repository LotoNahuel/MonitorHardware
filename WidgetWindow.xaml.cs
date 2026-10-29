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
        }
        // List<string> miLista = new List<string>();
        private void ActualizarSensores(bool bol, List<string> miLista)
        {
            if (bol != false)
            {
                // foreach (var sensor in SensorValues)
                // {
                //     var sensorGet = values.GetValueSensors(sensor.NameSensor);
                //     string valor = $"{sensorGet.value} {TSensor(sensorGet.type)}";

                //     sensor.ValueSensor = valor;
                // }
                int i = 0;
                var sensorGet = values.GetValueSensors(miLista);
                while (i < SensorValues.Count)
                {
                    var sensor = SensorValues[i];
                    string valor = $"{sensorGet[i].value} {TSensor(sensorGet[i].type)}";
                    
                    sensor.ValueSensor = valor;

                    i += 1;
                }
            }
        }
        private readonly DispatcherTimer timer = new();
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

                List<OverlayDisplaySensorItem> name = new List<OverlayDisplaySensorItem>();
                
                string filePath = Path.Combine(AppContext.BaseDirectory,"sensor-config.json");
                string jsonString = File.ReadAllText(filePath);
                List<SensorJson> Sensors = JsonSerializer.Deserialize<List<SensorJson>>(jsonString) ?? new List<SensorJson>();
                
                List<string> miLista = new List<string>();
                if (Sensors != null)
                {
                    foreach (var sensor in Sensors)
                    {
                        miLista.Add(sensor.NameSensor);
                    }
                    var sensorGet = values.GetValueSensors(miLista);
                    for (int i = 0; i < Sensors.Count && i < sensorGet.Count; i++)
                    {
                        var sensor = Sensors[i];
                        SensorValues.Add(new OverlayDisplaySensorItem
                        {
                            NameSensor = $"{sensor.NameSensor}",
                            ValueSensor = $"{sensorGet[i].value} {TSensor(sensorGet[i].type)}"
                        });
                    }
                }
                timer.Interval = TimeSpan.FromSeconds(0.5);
                timer.Tick += (sender, e) => ActualizarSensores(true, miLista);
                timer.Start();
            }
        }
    }
}