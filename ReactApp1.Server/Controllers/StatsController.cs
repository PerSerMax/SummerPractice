using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class StatsController : ControllerBase
{
    private readonly List<Measurement> _measurements;

    public StatsController(List<Measurement> measurements)
    {
        _measurements = measurements;
    }

    [HttpPost("upload")]
    public IActionResult UploadCsv(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("No file uploaded");
        }

        try
        {
            using var stream = file.OpenReadStream();
            using var reader = new StreamReader(stream);

            reader.ReadLine();

            var newMeasurements = new List<Measurement>();

            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
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
                    newMeasurements.Add(m);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error parsing line: {line}. Reason: {ex.Message}");
                }
            }

            _measurements.Clear();
            _measurements.AddRange(newMeasurements);

            return Ok(new { Count = newMeasurements.Count });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpGet]
    public IActionResult GetStats()
    {
        return Ok(StatisticsCalculator.CalculateStats(_measurements));
    }

    [HttpGet("by-type")]
    public IActionResult GetStatsByType([FromQuery] string type)
    {
        var filtered = _measurements.Where(m => m.Type.Equals(type, StringComparison.OrdinalIgnoreCase)).ToList();
        return Ok(StatisticsCalculator.CalculateStats(filtered));
    }

    [HttpGet("by-channel")]
    public IActionResult GetStatsByChannel([FromQuery] long channel)
    {
        var filtered = _measurements.Where(m => m.ChannelNumber == channel).ToList();
        return Ok(StatisticsCalculator.CalculateStats(filtered));
    }

    [HttpGet("by-temperature")]
    public IActionResult GetStatsByTemperature([FromQuery] double temp)
    {
        var filtered = _measurements.Where(m => Math.Abs(m.Temperature - temp) < 0.001).ToList();
        return Ok(StatisticsCalculator.CalculateStats(filtered));
    }
}