namespace ServiceMatch.Infrastructure.Options;

public class AcsOptions
{
    public string ConnectionString { get; set; } = "";
    public string SenderAddress { get; set; } = "no-reply@servicematch.dk";
}
