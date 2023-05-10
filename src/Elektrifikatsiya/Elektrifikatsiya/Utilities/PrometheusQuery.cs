using System.Net;
using System.Net.Sockets;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using Elektrifikatsiya.Models;
using FluentResults;

namespace Elektrifikatsiya.Utilities;

public class PrometheusQuery
{
    private readonly HttpClient client = new();

    public PrometheusQuery(string connectionString)
    {
        client.BaseAddress = new Uri(connectionString);
    }

    public  Task<PrometheusQueryResult?> Query(string query)
    {
        return client.GetFromJsonAsync<PrometheusQueryResult>($"/api/v1/query?query={UrlEncoder.Create().Encode(query)}", new JsonSerializerOptions() 
        {
            PropertyNameCaseInsensitive = true,
            Converters =
            {
                new JsonStringEnumConverter()
            }
        });
    }
}