using System.ComponentModel.DataAnnotations;

namespace NextCondoApi.Features.Configuration;

public class SMTPOptions
{
    public const string SMTP = "SMTP";

    [Required(AllowEmptyStrings = false)] 
    public string HOST {  get; set; } = String.Empty;
    [Required]
    public int PORT { get; set; }
    public string? USERNAME { get; set; }
    public string? PASSWORD { get; set; }
    [Required(AllowEmptyStrings = false)]
    public string DEFAULT_FROM {  get; set; } = String.Empty;
}
