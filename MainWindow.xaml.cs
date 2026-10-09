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

namespace MonitorHardware
{

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
    public partial class MainWindow :  Window
    {
        // public List<string> Hardware { get; set; } = new();
        // public List<string> Children { get; set; } = new();
        private void ArbolSensores_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.NewValue is HardwareSensorItem selectedItem)
            {
                // Handle the selected item here
                MessageBox.Show($"Selected: {selectedItem.HardwareName} - {selectedItem.SensorName}");
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
            // valueSensors values = new valueSensors();
            InitializeComponent();

            List<(string typeHardware, string nameHardware, string nameSensor, string typeSensor)> data = hardware.GetSensors();

            
            foreach (var (typeHardware, nameHardware, nameSensor, typeSensor) in data)
            {
                bool contH = HardwareSensorTreeView.Items.Cast<HardwareSensorItem>().Any(item => item.HardwareName == typeHardware);
                // bool contH = Hardware.Contains(typeHardware);
                if (contH is false)
                {
                    HardwareSensorItem hardwareItem = new HardwareSensorItem { HardwareName = typeHardware };
                    HardwareSensorTreeView.Items.Add(hardwareItem);
                }
                
                bool contS = HardwareSensorTreeView.Items.Cast<HardwareSensorItem>()
                    .SelectMany(item => item.Children)
                    .Any(child => child.SensorName == nameSensor);
                // bool contS = Children.Contains(nameHardware);
                if (contS is false)
                {
                    HardwareSensorItem hardwareItem = HardwareSensorTreeView.Items.Cast<HardwareSensorItem>().FirstOrDefault(item => item.HardwareName == typeHardware);
                    if (hardwareItem != null)
                    {
                        hardwareItem.Children.Add(new HardwareSensorItem { HardwareName = $"{nameSensor} ({typeSensor})" });
                        // hardwareItem.Children.Add(new HardwareSensorItem { SensorName = typeSensor });
                    }

                    // HardwareSensorItem.Children.Add(new HardwareSensorItem { SensorName = typeSensor });
                }
                
                // Sensores = new List<string>
                // {
                //     $"{typeHardware}"
                // };
            }
            DataContext = this;
        }
    }
}