using System;
using exportHardwareSensors;
using exportValueHardwareSensors;

namespace Program
{
    class ProgramX2
    {
        static void MainWindow()
        {
            hardwareSensors hardware = new hardwareSensors();
            valueSensors values = new valueSensors();
            // Dictionary<string, List<(string nameHardware, string nameSensor, string typeSensor)>> data = hardware.GetSensors();
            List<(string nameHardware, string nameSensor, string typeSensor)> data = hardware.GetSensors();

            foreach (var sensor in data)
            {
                if (sensor.typeSensor == "Temperature")
                {
                    // Console.Clear();
                    string valor = values.GetValueSensors(sensor.nameSensor);
                    // Console.WriteLine($"\t{sensor.nameHardware} \n\t\t{sensor.nameSensor} - {valor} \t{sensor.typeSensor}");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine($"\t{sensor.nameHardware}");
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.Write($"\t\t{sensor.nameSensor}");
                    while (true)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write($"\r\t{valor}");
                    }
                }
            }
        }
    }
}

namespace ListSensorConfig
{
    class ListConfig
    {
        public string NameHardware { get; set; }
        public string NameSensor { get; set; }
        public string TypeSensor { get; set; }
    }

    
}