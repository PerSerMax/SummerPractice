
public struct Measurement
{
    public string Type { get; set; }
    public long SerialNumber { get; set; }
    public long TestId { get; set; }
    public DateTime TimeStamp { get; set; }
    public long ProductId { get; set; }
    public long ChannelNumber { get; set; }
    public double THD { get; set; }
    public double Temperature { get; set; }
}
