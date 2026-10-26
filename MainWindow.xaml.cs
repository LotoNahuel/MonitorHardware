using System;
using System.IO;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using exportHardwareSensors;
using System.Text.Json;
using saveJson;
using System.Security.Cryptography.X509Certificates;
using OverlayMonitorHardware;

namespace MonitorHardware
{
    public partial class MainWindow : Window
    {
        public List<string> Hardware { get; } = new();
        hardwareSensors hardware = new hardwareSensors();

        private async void button_Click(object sender, RoutedEventArgs e)
        {
            List<ListConfig> sensorConfigurations = new List<ListConfig>();

            foreach (var item in HardwareSensorTreeView.Items)
            {
                if (item is HardwareSensorItem hardwareItem)
                {

                    foreach (var child in hardwareItem.Children)
                    {
                        if (child.IsChecked)
                        {
                            sensorConfigurations.Add(new ListConfig
                            {
                                NameHardware = hardwareItem.HardwareName,
                                NameSensor = child.SensorName,
                                TypeSensor = child.HardwareName.Split('(').Length > 1 ? child.HardwareName.Split('(')[1].TrimEnd(')') : ""
                            });
                        }
                    }
                }
            }

            await configurationMonitor.SaveConfiguration(sensorConfigurations);

            MessageBox.Show("Configuración guardada en sensor-config.json.");
        }

        private void SensorCheckBox_Click(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox checkBox &&
                ItemsControl.ContainerFromElement(HardwareSensorTreeView, checkBox) is TreeViewItem treeViewItem)
            {
                if (sender is CheckBox checkBox2 && checkBox2.DataContext is HardwareSensorItem sensorItem)
                {
                    if (sensorItem.IsChecked)
                        treeViewItem.IsSelected = false;
                    else
                        treeViewItem.IsSelected = true;
                }
                
            }
            
        }

        public class HardwareSensorItem : INotifyPropertyChanged
        {
            private bool _isChecked;
            public string HardwareName { get; set; } = "";
            public string SensorName { get; set; } = "";
            public List<HardwareSensorItem> Children { get; set; } = new();

            public bool IsChecked
            {
                get => _isChecked;
                set
                {
                    if (_isChecked == value)
                        return;

                    _isChecked = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsChecked)));

                    foreach (HardwareSensorItem child in Children)
                    {
                        child.IsChecked = value;
                    }
                }
            }
            public event PropertyChangedEventHandler? PropertyChanged;
        };

        private async Task<List<ListConfig>> CargarConfiguracion()
        {
            string filePath = Path.Combine(
                AppContext.BaseDirectory,
                "sensor-config.json"
            );

            if (!File.Exists(filePath))
                return new List<ListConfig>();

            string jsonString = await File.ReadAllTextAsync(filePath);

            return JsonSerializer.Deserialize<List<ListConfig>>(jsonString) ?? new List<ListConfig>();
        }

        private void RestaurarTildes(
            HardwareSensorItem item,
            List<ListConfig> configuracion)
        {
            if (!string.IsNullOrEmpty(item.SensorName))
            {
                bool guardado = configuracion.Any(s =>
                    s.NameSensor == item.SensorName ||
                    // IS NOT NECESARY THE COMPARATION DE NameHardware WITH HardawareName
                    s.NameHardware == item.HardwareName
                );
                item.IsChecked = guardado;
            }

            foreach (HardwareSensorItem child in item.Children)
            {
                RestaurarTildes(child, configuracion);
            }
        }

        private async void MainWindow_Loaded(
            object sender,
            RoutedEventArgs e)
        {
            List<ListConfig> configuracion = await CargarConfiguracion();

            foreach (HardwareSensorItem item in HardwareSensorTreeView.Items)
            {
                RestaurarTildes(item, configuracion);
            }
        }

        public MainWindow()
        {
            hardware = new hardwareSensors();
            InitializeComponent();

            List<(string typeHardware, string nameHardware, string nameSensor, string typeSensor)> data = hardware.GetSensors();

            foreach (var (typeHardware, nameHardware, nameSensor, typeSensor) in data)
            {
                bool contH = HardwareSensorTreeView.Items.Cast<HardwareSensorItem>()
                    .Any(item => item.HardwareName == nameHardware);
                if (!contH)
                    HardwareSensorTreeView.Items.Add(new HardwareSensorItem { HardwareName = nameHardware });

                bool contS = HardwareSensorTreeView.Items.Cast<HardwareSensorItem>()
                    .SelectMany(item => item.Children)
                    .Any(child => child.SensorName == nameSensor);

                if (!contS)
                {
                    HardwareSensorItem? hardwareItem = HardwareSensorTreeView.Items.Cast<HardwareSensorItem>()
                        .FirstOrDefault(item => item.HardwareName == nameHardware);

                    hardwareItem?.Children.Add(new HardwareSensorItem
                    {
                        HardwareName = $"{nameSensor} ({typeSensor})",
                        SensorName = nameSensor
                    });
                }
            }
            
            DataContext = this;
            Loaded += MainWindow_Loaded;
        }

        private WidgetWindow? widgetWindow;
    
        private void OpenWidget_Click(
            object sender,
            RoutedEventArgs e)
        {
            if ( widgetWindow == null || !widgetWindow.IsLoaded )
            {
                widgetWindow = new WidgetWindow();
                widgetWindow.Show();
            }
            else
            {
                widgetWindow.Activate();
            }
        }

        private void CloseWidget(
            object sender,
            RoutedEventArgs e)
        {
            if ( widgetWindow != null && widgetWindow.IsLoaded )
            {
                widgetWindow.Close();
                widgetWindow = null;
            }
        }
    }
}