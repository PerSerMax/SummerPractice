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

