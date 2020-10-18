using SpikeLib;
using SpikeLib.Messages;
using System;
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

            SpikeHub hub = new SpikeHub("COM8");
            await hub.OpenAsync();
            


            while (true)
            {
                var val = await hub.ReadMessageAsync();
                if (val == null)
                {
                    return;
                }
                if (val is BatteryMessage || val is PortStatusMessage) continue;
                Console.WriteLine(val);
                ;
            }

        }
    }
}
