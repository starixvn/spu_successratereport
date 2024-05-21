using System.Net.Http.Headers;
using System.Net.Http.Json;
using SuccessRateReport.Extensions;
using SuccessRateReport.Models;
using SuccessRateReport.Models.Responses;

var httpClient = new HttpClient()
{
    BaseAddress = new Uri("http://elastic.inf.owgps.net"),
    DefaultRequestHeaders =
    {
         Authorization = new AuthenticationHeaderValue("Basic", "ZWxhc3RpYzpwMGFseEA5ZGxj")
    }
};

var requestBody = await File.ReadAllTextAsync("RequestBody.json");
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

await File.WriteAllTextAsync("SuccessRate.csv", successRate);