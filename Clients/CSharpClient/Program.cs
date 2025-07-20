using AV.Engine.Core.Interfaces;
using AV.Engine.DI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var host = Host.CreateDefaultBuilder(args)
            .ConfigureServices((context, services) =>
            {
                services.AddAVEngineServices();
            })
            .Build();

        var engine = host.Services.GetRequiredService<IAVEngine>();

        engine.ThreatDetected += (sender, e) =>
            Console.WriteLine($"Threat detected: {e.Threat} via {e.ScanType} scan");

        engine.ScanStarted += (sender, e) =>
            Console.WriteLine($"Scan started: {e.ScanId}");

        engine.ScanStopped += (sender, e) =>
            Console.WriteLine($"Scan stopped: {e.ScanId} - {e.Reason}");

        engine.RealTimeScanStatusChanged += (sender, e) =>
            Console.WriteLine($"Real-time scanning: {(e.IsEnabled ? "Enabled" : "Disabled")}");

        Console.WriteLine("AV Engine Console Client");
        Console.WriteLine("Commands: 'r' - toggle real-time, 's' - start scan, 'x' - stop scan, 'e' - show events, 'c' - clear events, 'q' - quit");

        ConsoleKeyInfo key;
        do
        {
            key = Console.ReadKey(true);

            try
            {
                switch (key.KeyChar)
                {
                    case 'r':
                        if (engine.IsRealTimeEnabled)
                            await engine.DisableRealTimeAsync();
                        else
                            await engine.EnableRealTimeAsync();
                        break;

                    case 's':
                        var result = await engine.StartOnDemandScanAsync();
                        Console.WriteLine($"Scan start result: {result}");
                        break;

                    case 'x':
                        var stopped = await engine.StopOnDemandScanAsync();
                        Console.WriteLine($"Scan stop result: {stopped}");
                        break;

                    case 'e':
                        var events = await engine.GetPersistedEventsAsync();
                        Console.WriteLine($"Found {events.Count} persisted events:");
                        foreach (var evt in events)
                            Console.WriteLine($"  {evt.Timestamp:HH:mm:ss} - {evt.EventType}");
                        break;

                    case 'c':
                        await engine.ClearPersistedEventsAsync();
                        Console.WriteLine("Events cleared");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

        } while (key.KeyChar != 'q');

        engine.Dispose();
        Console.WriteLine("Goodbye!");
    }
}