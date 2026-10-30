using System.Linq;
using System.Reflection.Metadata;
using System.Windows;
using System.Windows.Automation.Peers;
using exportHardwareSensors;
using LibreHardwareMonitor.Hardware;
using LibreHardwareMonitor.Interop.PowerMonitor;

// while (true)
// {
// var miClase = new MiClase();
// miClase.Test();

namespace exportHardwareSensors
{
    public class hardwareSensors
    {
        public List<(string typeHardware, string nameHardware, string nameSensosr, string typeSensor)> GetSensors()
        {
            Computer computer = new Computer
            {
                IsCpuEnabled = true,
                IsGpuEnabled = true,
                IsMemoryEnabled = true,
                IsMotherboardEnabled = true,
                IsControllerEnabled = true,
                IsNetworkEnabled = true, 
                IsStorageEnabled = true,
                IsPowerMonitorEnabled = true,
            };

            computer.Open();
            computer.Accept(new UpdateVisitor());

            // Dictionary<string, List<(string nameHardware, string nameSensor, string typeSensor)>> data_hardware = new();
            List<(string typeHardware, string nameHardware, string nameSensor, string typeSensor)> data_hardware = new();

            foreach (IHardware hardware in computer.Hardware)
            {
                hardware.Update();
                // if (
                //     hardware.HardwareType == HardwareType.GpuNvidia ||
                //     hardware.HardwareType == HardwareType.GpuAmd ||
                //     hardware.HardwareType == HardwareType.GpuIntel ||
                //     hardware.HardwareType == HardwareType.Cpu
                // )
                // {
                    // Console.WriteLine($"\nHardware [{hardware.Name}]");
                    // Console.WriteLine($"{hardware.HardwareType}");
                    // Console.WriteLine("Hardware: {0}", hardware.Name);

                    foreach (IHardware subhardware in hardware.SubHardware)
                    {
                        // Console.WriteLine("\tSubhardware: {0}", subhardware.Name);
                        
                        foreach (ISensor sensor in subhardware.Sensors)
                        {
                            data_hardware.Add(
                            (
                                $"{hardware.HardwareType}",
                                $"{hardware.Name}",
                                $"{sensor.Name}",
                                $"{sensor.SensorType}"
                            )
                        );
                            // Console.WriteLine("\t-------------------------------------------------------------------------");
                            // Console.WriteLine("\t\tSensor: {0}, value: {1}, is: {2}", sensor.Name, sensor.Value ?? 0, sensor.SensorType);
                        }
                    }
                    
                    hardware.Update();
                    
                    foreach (ISensor sensor in hardware.Sensors)
                    {
                        data_hardware.Add(
                            (
                                $"{hardware.HardwareType}",
                                $"{hardware.Name}",
                                $"{sensor.Name}",
                                $"{sensor.SensorType}"
                            )
                        );
                    }
                // }
            }
            return data_hardware;
        }
    }
        // Thread.Sleep(1000);
    // }
}

namespace exportValueHardwareSensors
{
    public class valueSensors
    {
        public List<(string name, string value, string type)> GetValueSensors(List<string> dataSensor)
        {
            Computer computer = new Computer
            {
                IsCpuEnabled = true,
                IsGpuEnabled = true,
                IsMemoryEnabled = true,
                IsMotherboardEnabled = true,
                IsControllerEnabled = true,
                IsNetworkEnabled = true,
                IsStorageEnabled = true,
                IsPowerMonitorEnabled = true,
            };

            computer.Open();
            computer.Accept(new UpdateVisitor());
            var ListTuple = new List<(string, string, string)>();
            // List <string> values = new();
            foreach (IHardware hardware in computer.Hardware)
            {
                hardware.Update();

                int i = 0;
                
                while (i < dataSensor.Count)
                {
                    ISensor? valor = hardware.Sensors.FirstOrDefault(p => p.Name == dataSensor[i]);
                    
                    if (valor != null)
                    {
                        int indice = ListTuple.FindIndex(p => p.Item1 == valor.Name);
                        // MessageBox.Show($"{namme.Item1} \n {valor.Name}");
                        // namme.Item1 = valor.Name;
                        if (indice != -1)
                        {
                            ListTuple[indice] = (
                                valor.Name,
                                valor.Value?.ToString("F2") ?? "N/A",
                                valor.SensorType.ToString()
                            );
                        }
                        else
                        {
                            ListTuple.Add(($"{valor.Name}", $"{valor.Value?.ToString("F2") ?? "N/A"}", $"{valor.SensorType}"));
                        }
                        
                        // namme.Item3 = valor.SensorType;
                        // values.Add($"{dataSensor[i]}: VALOR: {valor.Value?.ToString("F2")??"N/A"}\n");
                        // ListTuple.Add(($"{valor.Name}", $"{valor.Value?.ToString("F2") ?? "N/A"}", $"{valor.SensorType}"));
                    }
                    i += 1;
                }
            }
            // MessageBox.Show(string.Join(", ", values));
            return ListTuple;
        }
    }
}


public class UpdateVisitor : IVisitor
{
    public void VisitComputer(IComputer computer) => computer.Traverse(this);

    public void VisitHardware(IHardware hardware)
    {
        hardware.Update();
        foreach (IHardware subHardware in hardware.SubHardware)
            subHardware.Accept(this);
    }

    public void VisitSensor(ISensor sensor) { }

    public void VisitParameter(IParameter parameter) { }
}