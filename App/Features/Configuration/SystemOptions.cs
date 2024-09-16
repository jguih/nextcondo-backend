using System.ComponentModel.DataAnnotations;

namespace NextCondoApi.Features.Configuration;

public class SystemOptions
{
    public const string SYSTEM = "SYSTEM";

    [Url]
    public string? PUBLIC_URL {  get; set; }
}
