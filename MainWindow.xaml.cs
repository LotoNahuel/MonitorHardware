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
        public List<string> Hardware { get; set; } = new();
        public List<string> Children { get; set; } = new();

        public MainWindow()
        {
            hardwareSensors hardware = new hardwareSensors();
            valueSensors values = new valueSensors();
            InitializeComponent();

            List<(string typeHardware, string nameHardware, string nameSensor, string typeSensor)> data = hardware.GetSensors();

            
            foreach (var (typeHardware, nameHardware, nameSensor, typeSensor) in data)
            {
                bool contH = Hardware.Contains(typeHardware);
                if (contH is false)
                {
                    Hardware.Add(typeHardware);
                }
                
                bool contS = Children.Contains(nameHardware);
                if (contS is false)
                {
                    Children.Add(nameHardware);
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