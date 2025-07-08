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
    var filename = "Data/test.csv"; // Путь к файлу относительно проекта сервера

    if (!System.IO.File.Exists(filename)) {
        return;
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
    await context.Response.WriteAsJsonAsync(measurements);
});

app.Run();
