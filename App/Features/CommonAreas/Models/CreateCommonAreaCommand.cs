
using System.ComponentModel.DataAnnotations;

namespace NextCondoApi.Features.CommonAreasFeature.Models;

public class CreateCommonAreaCommand
{
    [Required(AllowEmptyStrings = false)]
    public required string Name { get; set; }
    [Required(AllowEmptyStrings = false)]
    public required string Description { get; set; }
    [Required]
    public TimeOnly StartTime { get; set; }
    [Required]
    public TimeOnly EndTime { get; set; }
    [Required]
    public TimeOnly TimeInterval { get; set; }
}