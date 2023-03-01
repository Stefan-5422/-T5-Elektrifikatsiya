using System.Text.Json.Serialization;

namespace Elektrifikatsiya.Models;

public enum Status
{
    Success,
    Error
}

public enum ResultType
{
    Matrix,
    Vector,
    Scalar,
    String
}

public class PrometheusQueryResult
{
    public Status Status { get; set; }

    public string? ErrorType { get; set; }
    public string? Error { get; set; }
    public List<string>? Warnings { get; set; }

    public PrometheusQueryResult(Status status, string? errorType, string? error, List<string>? warnings)
    {
        Status = status;
        ErrorType = errorType;
        Error = error;
        Warnings = warnings;
    }
}

internal class PrometheusDataWrapper
{
    public ResultType ResultType { get; set; }
    public List<PrometheusData> Result { get; set; }

    public PrometheusDataWrapper(ResultType resultType, List<PrometheusData> result)
    {
        ResultType = resultType;
        Result = result;
    }
}

internal class PrometheusDataMetric
{
    [JsonPropertyName("__name__")] public string Name { get; set; }

    public string Job { get; set; }
    public string Instance { get; set; }

    public PrometheusDataMetric(string name, string job, string instance)
    {
        Name = name;
        Job = job;
        Instance = instance;
    }
}

internal class PrometheusData
{
    public PrometheusDataMetric Metric { get; set; }
    public object Value { get; set; }

    public PrometheusData(PrometheusDataMetric metric, object value)
    {
        Metric = metric;
        Value = value;
    }
}