public class BinInfo
{
    public string Scheme { get; set; }
    public string Type { get; set; }
    public string Brand { get; set; }
    public BankInfo Bank { get; set; }
    public CountryInfo Country { get; set; }
}

public class BankInfo
{
    public string Name { get; set; }
    public string Url { get; set; }
}

public class CountryInfo
{
    public string Name { get; set; }
    public string Currency { get; set; }
}
