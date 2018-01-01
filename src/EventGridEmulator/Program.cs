using EventGridEmulator.Contracts;
using EventGridEmulator.Logic;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;

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

                var globalEventQueue = new EventQueue();
                var consoleLogger = new ConsoleLogger();
                var subscriptionLookup = new SubscriptionLookup(globalEventQueue, consoleLogger);

                //Start listening for incoming events.
                var listener = new EventListener(config, subscriptionLookup, consoleLogger);
                var t1 = listener.StartListeningAsync();

                //Starts watching the event queue.
                var dispatcher = new EventDispatcher(config, globalEventQueue, consoleLogger);
                var t2 = dispatcher.StartListening();

                Task.WaitAll(t1, t2);
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
