using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using exportHardwareSensors;
using exportValueHardwareSensors;
using LibreHardwareMonitor.Hardware;

namespace MonitorHardware
{

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
    public partial class MainWindow :  Window
    {
        public List<string> Hardware { get; } = new();
        // public List<string> Children { get; } = new();
        private void ArbolSensores_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            hardwareSensors hardware = new hardwareSensors();
            List<(string typeHardware, string nameHardware, string nameSensor, string typeSensor)> data = hardware.GetSensors();
            if (e.NewValue is HardwareSensorItem selectedItem)
            {
                bool isSelected = data.Any(item => item.nameHardware == selectedItem.HardwareName);
                // foreach (var (typeHardware, nameHardware, nameSensor, typeSensor) in data)
                // {
                    if (isSelected)
                    {
                        MessageBox.Show($"Selected: {selectedItem.HardwareName}");
                    }
                    // Handle the selected item here
                // MessageBox.Show($"Selected: {selectedItem.HardwareName} {selectedItem.SensorName}");
                // }
                
            }
        }

        private void SensorCheckBox_Click(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox checkBox &&
                ItemsControl.ContainerFromElement(HardwareSensorTreeView, checkBox) is TreeViewItem treeViewItem)
            {
                // Clicking the CheckBox does not select its TreeViewItem automatically.
                treeViewItem.IsSelected = true;
            }
        }

        
        public class HardwareSensorItem
        {
            public string HardwareName { get; set; } = "";
            public string SensorName { get; set; } = "";
            public List<HardwareSensorItem> Children { get; set; } = new();
        }

        public MainWindow()
        {
            hardwareSensors hardware = new hardwareSensors();
            InitializeComponent();

            List<(string typeHardware, string nameHardware, string nameSensor, string typeSensor)> data = hardware.GetSensors();

            foreach (var (typeHardware, nameHardware, nameSensor, typeSensor) in data)
            {
                bool contH = HardwareSensorTreeView.Items.Cast<HardwareSensorItem>().Any(item => item.HardwareName == nameHardware);
                if (contH is false)
                {
                    HardwareSensorItem hardwareItem = new HardwareSensorItem { HardwareName = nameHardware };
                    HardwareSensorTreeView.Items.Add(hardwareItem);
                }
                
                bool contS = HardwareSensorTreeView.Items.Cast<HardwareSensorItem>()
                    .SelectMany(item => item.Children)
                    .Any(child => child.SensorName == nameSensor);
                // bool contS = Children.Contains(nameHardware);
                if (contS is false)
                {
                    HardwareSensorItem hardwareItem = HardwareSensorTreeView.Items.Cast<HardwareSensorItem>().FirstOrDefault(item => item.HardwareName == nameHardware);
                    if (hardwareItem != null)
                    {
                        HardwareSensorItem sensorItem = new HardwareSensorItem
                        {
                            HardwareName = $"{nameSensor} ({typeSensor})",
                            SensorName = nameSensor
                        };
                        hardwareItem.Children.Add(sensorItem);
                    }
                }
            }
            DataContext = this;
        }
    }
}
