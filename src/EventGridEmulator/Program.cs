using EventGridEmulator.Contracts;
using Newtonsoft.Json;
using System;
using System.IO;

namespace EventGridEmulator
{
    class Program
    {
        private const string ConfigFilePath = "emulator-config.json";

        static void Main(string[] args)
        {
            Console.WriteLine("Unofficial Azure EventGrid Emulator");
            Console.WriteLine("by Ravin Perera");
            Console.WriteLine();

            try
            {
                var config = ReadConfigurationFromFile();

                var listener = new EventListener(config, new ConsoleLogger());
                listener.StartListeningAsync().Wait();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
            }

            Console.WriteLine("Done");
            Console.Read();
        }

        static EmulatorConfiguration ReadConfigurationFromFile()
        {
            var json = File.ReadAllText(ConfigFilePath);
            return JsonConvert.DeserializeObject<EmulatorConfiguration>(json);
        }
    }
}
