using SpikeLib;
using SpikeLib.Messages;
using System;
using System.IO;
using System.IO.Ports;
using System.Management;
using System.Threading.Tasks;

namespace SpikeComm
{
    class Program
    {


        static async Task Main(string[] args)
        {
            //var portNames = SerialPort.GetPortNames();
            //ManagementClass objInst = new ManagementClass("Win32_SerialPort");
            //var instances = objInst.GetInstances();

            //foreach (var inst in instances)
            //{
            //    Console.WriteLine(inst.ClassPath);
            //    foreach (var prop in inst.Properties)
            //    {
            //        Console.WriteLine(prop.Name + " : " + prop.Value);
            //    }
            //}
            //;

            SpikeHub hub = new SpikeHub("COM5");
            await hub.OpenAsync();

            // await Task.Delay(1000);

            var storage = await hub.RequestStorageAsync();
            Console.WriteLine(storage);

            {
                using var stream = File.OpenRead(@"C:\Users\thadh\Documents\LEGO MINDSTORMS\PythonYeet.py");
                await hub.UploadFileAsync(stream, 7, "HelloSharp");
            }

            var reader = hub.UnknownMessagesReader;

            while (true)
            {
                var val = await reader.ReadAsync();
                if (val is BatteryMessage || val is PortStatusMessage) continue;
                Console.WriteLine(val);
                ;
            }

        }
    }
}
