using System.Net;
using System.Net.Sockets;
using System.Text.Encodings.Web;
using System.Text.Json;
using Elektrifikatsiya.Models;
using FluentResults;

namespace Elektrifikatsiya.Utilities;

public class PrometheusQuery
{
    private string connectionString;
    private readonly HttpClient client = new();

    public PrometheusQuery(string connectionString)
    {
        this.connectionString = connectionString;
        client.BaseAddress = new Uri(connectionString);
    }

    public  Task<PrometheusQueryResult?> Query(string query)
    {
        return client.GetFromJsonAsync<PrometheusQueryResult>($"/v1/query?{UrlEncoder.Create().Encode(query)}");
    }
}