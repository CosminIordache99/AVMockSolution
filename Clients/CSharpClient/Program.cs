using AV.Engine;
using AV.Engine.Core.Interfaces;
using AV.SDK;

internal class Program
{
    private static async Task Main(string[] args)
    {
        using var client = new AVClient(new AVEngine());

        client.ScanStarted += (_, e) =>
             Console.WriteLine($"[Start] {e.Timestamp}");

        //client.ThreatsFound += (_, e) =>
        //    foreach (var t in e.Threats)
        //    Console.WriteLine($"[Threat] {t.FilePath} → {t.ThreatName}");

        client.ScanStopped += (_, e) =>
            Console.WriteLine($"[Stop] {e.Timestamp}, reason={e.Reason}");

        Console.WriteLine("Pornesc scan on-demand...");
        var res = await client.StartScanAsync();
        if (res == StartScanResult.AlreadyRunning)
            Console.WriteLine("Scan deja în desfășurare.");

        // Aștept 5s și opresc
        await Task.Delay(5000);
        await client.StopScanAsync();

        // Citire evenimente persistente
        Console.WriteLine("Evenimente salvate:");
        foreach (var evt in client.GetEvents())
            Console.WriteLine(evt);
    }
}