using System.Diagnostics.Metrics;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

//app.UseDefaultFiles();
//app.UseStaticFiles();

//// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

//app.UseHttpsRedirection();

//app.UseAuthorization();

//app.MapControllers();

//app.MapFallbackToFile("/index.html");

app.Run(async (context) =>
{
    var measurements = ReadCsv("Data/test.csv");
    if (measurements != null)
        await context.Response.WriteAsJsonAsync(measurements);
    else
        context.Response.StatusCode = 500;
        await context.Response.WriteAsync("File not found!");
});

app.Run();


List<Measurement>? ReadCsv(string filename)
{
    if (!System.IO.File.Exists(filename))
    {
        Console.WriteLine("Файл не найден!");
        return null;
    }

    var lines = System.IO.File.ReadLines(filename);
    var measurements = new List<Measurement>();

    foreach (var line in lines.Skip(1)) // пропускаем заголовок
    {
        var fields = line.Split(';');
        try
        {
            var m = new Measurement
            {
                Type = fields[0],
                SerialNumber = long.Parse(fields[1]),
                TestId = long.Parse(fields[2]),
                TimeStamp = DateTime.Parse(fields[3]),
                ProductId = long.Parse(fields[4]),
                ChannelNumber = long.Parse(fields[5]),
                THD = double.Parse(fields[6]),
                Temperature = double.Parse(fields[7])
            };
            measurements.Add(m);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка парсинга строки: {line}. Причина: {ex.Message}");
        }
    }
    return measurements;
}

public class StatisticsCalculator
{
    public static double? CalculateAverage(List<Measurement> measurements, Func<Measurement, double> selector)
    {
        if (measurements == null || measurements.Count == 0)
        {
            Console.WriteLine("Нет данных для расчета среднего");
            return null;
        }

        return measurements.Average(selector);
    }

    public static double? CalculateMedian(List<Measurement> measurements, Func<Measurement, double> selector)
    {
        if (measurements == null || measurements.Count == 0)
        {
            Console.WriteLine("Нет данных для расчета медианы");
            return null;
        }

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

    public static List<double> CalculateMode(List<Measurement> measurements, Func<Measurement, double> selector)
    {
        if (measurements == null || measurements.Count == 0)
        {
            Console.WriteLine("Нет данных для расчета моды");
            return null;
        }

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