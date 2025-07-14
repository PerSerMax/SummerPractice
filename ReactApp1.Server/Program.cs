using System.Diagnostics.Metrics;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<List<Measurement>>(new List<Measurement>());

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();

public class StatisticsResponse
{
    public double? AverageTHD { get; set; }
    public double? MedianTHD { get; set; }
    public List<double>? ModeTHD { get; set; }
    public double? AverageTemperature { get; set; }
    public double? MedianTemperature { get; set; }
    public List<double>? ModeTemperature { get; set; }
    public int Count { get; set; }
}

public static class StatisticsCalculator
{
    public static StatisticsResponse CalculateStats(List<Measurement> measurements)
    {
        if (measurements == null || measurements.Count == 0)
        {
            return new StatisticsResponse { Count = 0 };
        }

        return new StatisticsResponse
        {
            AverageTHD = measurements.Average(m => m.THD),
            MedianTHD = CalculateMedian(measurements, m => m.THD),
            ModeTHD = CalculateMode(measurements, m => m.THD),
            AverageTemperature = measurements.Average(m => m.Temperature),
            MedianTemperature = CalculateMedian(measurements, m => m.Temperature),
            ModeTemperature = CalculateMode(measurements, m => m.Temperature),
            Count = measurements.Count
        };
    }

    private static double? CalculateMedian(List<Measurement> measurements, Func<Measurement, double> selector)
    {
        var values = measurements.Select(selector).OrderBy(v => v).ToList();
        int count = values.Count;

        if (count % 2 == 0)
        {
            return (values[count / 2 - 1] + values[count / 2]) / 2.0;
        }
        else
        {
            return values[count / 2];
        }
    }

    private static List<double> CalculateMode(List<Measurement> measurements, Func<Measurement, double> selector)
    {
        var groups = measurements
            .Select(selector)
            .GroupBy(v => v)
            .OrderByDescending(g => g.Count())
            .ToList();

        if (!groups.Any())
            return new List<double>();

        int maxCount = groups.First().Count();

        return groups
            .Where(g => g.Count() == maxCount)
            .Select(g => g.Key)
            .ToList();
    }
}