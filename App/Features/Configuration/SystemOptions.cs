using System.ComponentModel.DataAnnotations;

namespace NextCondoApi.Features.Configuration;

public class SystemOptions
{
    public const string SYSTEM = "SYSTEM";

    [Required(AllowEmptyStrings = false)]
    [Url]
    public string PUBLIC_URL {  get; set; } = String.Empty;
}
