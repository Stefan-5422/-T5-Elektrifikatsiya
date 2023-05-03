using System.Diagnostics;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.Diagnostics;

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

    public PrometheusDataWrapper? Data { get; set; }

    public PrometheusQueryResult(Status status, string? errorType, string? error, List<string>? warnings)
    {
        Status = status;
        ErrorType = errorType;
        Error = error;
        Warnings = warnings;
    }
}

public class PrometheusDataWrapper
{
    public ResultType ResultType { get; set; }
    public List<PrometheusData> Result { get; set; }

    public PrometheusDataWrapper(ResultType resultType, List<PrometheusData> result)
    {
        ResultType = resultType;
        Result = result;
    }
    
    public FluentResults.Result<List<(double, double)>> MatrixTypeToTimestampFloatTuple()
    {
	    if (ResultType != ResultType.Matrix)
	    {
		    return FluentResults.Result.Fail("The response did not have the Matrix Type");
	    }

        List<(double, double)> result = new List<(double, double)>();

        if (Result.Count == 0 || Result[0]?.Values is null || Result[0]?.Values?[0] is null)
        {
	        return FluentResults.Result.Fail("There was no result in the Response Body");
        }

        foreach (object value in Result[0]!.Values!)
	    {

		    string[] segment = value.ToString()!.Split(",");

            result.Add((Convert.ToDouble(segment[0][2..^1]), Convert.ToDouble(segment[1][1..^2])));
	    }
        Debug.WriteLine(result);
        return result;
    }

    public FluentResults.Result<(double, double)> VectorTypeToTimestampFloatTuple()
    {
	    if (ResultType != ResultType.Vector)
	    {
		    return FluentResults.Result.Fail("The response did not have the Vector Type");
	    }

	    string[]? segment = Result.FirstOrDefault()?.Value.ToString()?.Split(",") ?? null;

	    if (segment is null)
	    {
		    return FluentResults.Result.Fail("There was no result in the Response Body");
	    }

	    return((Convert.ToDouble(segment[0][2..^1]), Convert.ToDouble(segment[1][1..^2])));
    }
}


public class PrometheusDataMetric
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

public class PrometheusData
{
    public PrometheusDataMetric? Metric { get; set; } = null;
    public object? Value { get; set; } = null;
    public List<object>? Values { get; set; } = null;
}