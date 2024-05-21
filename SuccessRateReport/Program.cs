using System.Net.Http.Headers;
using System.Net.Http.Json;
using SuccessRateReport.Extensions;
using SuccessRateReport.Models;
using SuccessRateReport.Models.Responses;

Console.WriteLine("Only the following time formats are supported: 2024-05-01T00:00:00 or 2024-05-01T00:00:00-04:00");
Console.WriteLine("Please enter start time: ");
var startTime = Console.ReadLine()?.Trim();

Console.WriteLine("Please enter end time: ");
var endTime = Console.ReadLine()?.Trim();

Console.WriteLine("Please enter licensee type: ");
var licType = Console.ReadLine()?.Trim();

var httpClient = new HttpClient()
{
    BaseAddress = new Uri("http://elastic.inf.owgps.net"),
    DefaultRequestHeaders =
    {
         Authorization = new AuthenticationHeaderValue("Basic", "ZWxhc3RpYzpwMGFseEA5ZGxj")
    }
};

var requestBody = (await File.ReadAllTextAsync("RequestBody"+ (!string.IsNullOrWhiteSpace(licType) ? "_" + licType : string.Empty) + ".json"))
    .Replace("{0}", startTime + "T12:00:00")
    .Replace("{1}", endTime + "T12:00:00");
var httpResponseMessage = await httpClient.PostAsync("logs-bettingresult-prod/_search", new StringContent(requestBody, null, "application/json"));

var elasticResponse = await httpResponseMessage.EnsureSuccessStatusCode().Content.ReadFromJsonAsync<ElasticResponse>();

var successRate = elasticResponse
    .TryGetBuckets()
    .Select(x => new SuccessRateModel()
    {
        BettingErrorMessage = x.key,
        Count = x.doc_count,
    })
    .ToCsv(",");

await File.WriteAllTextAsync("SuccessRate"+ (!string.IsNullOrWhiteSpace(licType) ? "_" + licType : string.Empty) + ".csv", successRate);