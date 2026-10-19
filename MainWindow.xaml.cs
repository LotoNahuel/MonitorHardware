using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using exportHardwareSensors;
using saveJson;

namespace MonitorHardware
{
    public partial class MainWindow : Window
    {
        public List<string> Hardware { get; } = new();
        hardwareSensors hardware = new hardwareSensors();

        private void ArbolSensores_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.NewValue is not HardwareSensorItem selectedItem)
                return;

            string tipo = selectedItem.Children.Count > 0 ? "Padre" : "Hijo";
            // MessageBox.Show($"Select Item: {tipo}");
            // if (tipo == "Padre")
            // {
            //     MessageBox.Show($"Seleccionado: {selectedItem.HardwareName}\nTipo: {tipo}\nContiene {selectedItem.Children.Count} sensores.");
            // }
            // else
            // {
            //     MessageBox.Show($"Seleccionado: {selectedItem.SensorName}\nTipo: {tipo}");
            // }
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            List<string> selectedSensors = new List<string>();
            List<ListConfig> sensorConfigurations = new List<ListConfig>();

            foreach (var item in HardwareSensorTreeView.Items)
            {
                if (item is HardwareSensorItem hardwareItem)
                {
                    if (hardwareItem.IsChecked)
                    {
                        // MessageBox.Show($"Hardware: {hardwareItem.HardwareName} está seleccionado.");
                        // selectedSensors.Add(hardwareItem.HardwareName);

                    }

                    foreach (var child in hardwareItem.Children)
                    {
                        if (child.IsChecked)
                        {
                            selectedSensors.Add(child.SensorName);
                        }
                    }
                }
            }
            // SelectedSensorTextBlock.Text = string.Join(", ", selectedSensors);
            // MessageBox.Show($"Sensores seleccionados: {string.Join(", ", selectedSensors)}");
            // List<ListConfig> sensorConfigurations = new List<ListConfig>();
        }

        private void SensorCheckBox_Click(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox checkBox &&
                ItemsControl.ContainerFromElement(HardwareSensorTreeView, checkBox) is TreeViewItem treeViewItem)
            {
                if (sender is CheckBox checkBox2 && checkBox2.DataContext is HardwareSensorItem sensorItem)
                {
                    // Actualizar el estado de IsChecked en el objeto HardwareSensorItem.
                    // if (sensorItem.IsChecked != checkBox2.IsChecked)
                    MessageBox.Show($"Sensor: {sensorItem.SensorName}\nIsChecked: {sensorItem.IsChecked}");
                    if (sensorItem.IsChecked)
                    {
                        treeViewItem.IsSelected = false;
                    }
                    else
                    {
                        treeViewItem.IsSelected = true; 
                    }
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
                        child.IsChecked = value;
                }
            }

            public event PropertyChangedEventHandler? PropertyChanged;
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
        }
    }
}
