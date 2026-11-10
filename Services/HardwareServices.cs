using System.Collections.Concurrent;
using System.ComponentModel;
using System.Linq;
using System.Reflection.Metadata;
using System.Windows;
using System.Windows.Automation.Peers;
using exportHardwareSensors;
using LibreHardwareMonitor.Hardware;
using LibreHardwareMonitor.Interop.PowerMonitor;

// public class MiServiceSecondPlane : BackgroundService
// {
//     protected override async Task ExecuteAsync(CancellationToken stoppingToken)
//     {
//         while (!stoppingToken.IsCancellationRequested)
//         {
//             Computer computer = new Computer
//             {
//                 IsCpuEnabled = true,
//                 IsGpuEnabled = true,
//                 IsMemoryEnabled = true,
//                 IsMotherboardEnabled = true,
//                 IsControllerEnabled = true,
//                 IsNetworkEnabled = true, 
//                 IsStorageEnabled = true,
//                 IsPowerMonitorEnabled = true,
//             };

//             computer.Open();
//             computer.Accept(new UpdateVisitor());
//         }
//     }
// }

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

                    foreach (IHardware subhardware in hardware.SubHardware)
                    {
                        
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
}

namespace exportValueHardwareSensors
{
    public class valueSensors : IDisposable
    {
        private readonly Computer computer;
        private readonly object lockSensors = new();

        public valueSensors()
        {
            computer = new Computer
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
        }
        public List<(string name, string value, string type)> GetValueSensors(List<string> dataSensor)
        {
            lock (lockSensors)
            {
                var ListTuple = new List<(string, string, string)>();

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
                        }
                        i += 1;
                    }
                }
                return ListTuple;
            }
        }

        public void Dispose()
        {
            lock (lockSensors)
            {
                computer.Close();
            }
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