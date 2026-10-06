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
// public partial class MainWindow : Window
    public partial class MainWindow :  Window
    {
        public List<string> Sensores { get; set; }
        // public List<string> Hardware { get; set; }

        public MainWindow()
        {
            hardwareSensors hardware = new hardwareSensors();
            valueSensors values = new valueSensors();
            InitializeComponent();
            while (true)
            {
                List<(string typeHardware, string nameHardware, string nameSensor, string typeSensor)> data = hardware.GetSensors();
                foreach (var sensor in data)
                {
                    // Hardware = new List<string>
                    // {
                    //     $"{sensor.typeHardware}"
                    // };
                    Sensores = new List<string>
                    {
                        $"{sensor.nameHardware}"
                    };
                }
                DataContext = this;
            }
            
        }
    }
}