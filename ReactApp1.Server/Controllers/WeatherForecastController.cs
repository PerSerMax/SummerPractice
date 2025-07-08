using Microsoft.AspNetCore.Mvc;

namespace ReactApp1.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetMeasurements")]
        public IEnumerable<Measurement> Get()
        {
            var filename = "Data/test.csv"; // Путь к файлу относительно проекта сервера

            if (!System.IO.File.Exists(filename))
                return Enumerable.Empty<Measurement>();

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
                    _logger.LogWarning($"Ошибка парсинга строки: {line}. Причина: {ex.Message}");
                }
            }

            return measurements;
        }
    }
}
