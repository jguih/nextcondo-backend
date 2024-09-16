using System.ComponentModel.DataAnnotations;

namespace NextCondoApi.Features.Configuration;

public class DbOptions
{
    public const string DB = "DB";

    [Required(AllowEmptyStrings = false)]
    public string HOST { get; set; } = String.Empty;
    [Required(AllowEmptyStrings = false)]
    public string DATABASE { get; set; } = String.Empty;
    [Required(AllowEmptyStrings = false)]
    public string USER { get; set; } = String.Empty;
    [Required(AllowEmptyStrings = false)]
    public string PASSWORD { get; set; } = String.Empty;
}
