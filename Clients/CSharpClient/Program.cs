// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");
var apiBase = Environment.GetEnvironmentVariable("API_BASE_URL")
              ?? "http://localhost:5046";

using var http = new HttpClient { BaseAddress = new Uri(apiBase) };
var json = await http.GetStringAsync("/WeatherForecast");
Console.WriteLine("Forecast: " + json);
