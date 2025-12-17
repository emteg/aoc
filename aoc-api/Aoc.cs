namespace AocApi;

public static class Aoc
{
    public static Stream GetDescription(ushort year, byte day, string cookie) 
    {
        string url = $"https://adventofcode.com/{year}/day/{day}";
        Console.Error.WriteLine($"Fetching puzzle from AOC ({url})...");
        
        HttpRequestMessage request = new(HttpMethod.Get, url);
        request.Headers.Add("Cookie", $"session={cookie}");
        
        HttpResponseMessage response = HtClient.SendAsync(request).Result.EnsureSuccessStatusCode();

        return response.Content.ReadAsStream();
    }
    
    public static Stream GetInput(ushort year, byte day, string cookie)
    {
        string url = $"https://adventofcode.com/{year}/day/{day}/input";
        Console.Error.WriteLine($"Fetching input from AOC ({url})...");
        
        HttpRequestMessage request = new(HttpMethod.Get, url);
        request.Headers.Add("Cookie", $"session={cookie}");
        
        HttpResponseMessage response = HtClient.SendAsync(request).Result.EnsureSuccessStatusCode();

        return response.Content.ReadAsStream();
    }
    
    public static string Submit(string cookie, ushort year, byte day, byte part, string answer)
    {
        string url = $"https://adventofcode.com/{year}/day/{day}/answer";
        Console.WriteLine($"Submitting '{answer}' to AOC ({url})...");
        HttpRequestMessage request = new(HttpMethod.Post, url);
        
        request.Headers.Add("Cookie", $"session={cookie}");
        
        request.Content = new FormUrlEncodedContent([
            new KeyValuePair<string, string>("level", part.ToString()!),
            new KeyValuePair<string, string>("answer", answer)
        ]);
        
        HttpResponseMessage response = HtClient.SendAsync(request).Result.EnsureSuccessStatusCode();
        string content = response.Content.ReadAsStringAsync().Result;

        return content;
    }
    
    private static readonly HttpClientHandler Handler = new() { UseCookies = false };
    private static readonly HttpClient HtClient = new(Handler);
}