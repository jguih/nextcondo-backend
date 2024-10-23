
using System.ComponentModel.DataAnnotations;

namespace NextCondoApi.Features.CommonAreasFeature.Models;

public class CreateCommonAreaCommand
{
    [Required]
    public required int TypeId { get; set; }
    [Required(AllowEmptyStrings = false)]
    public TimeOnly StartTime { get; set; }
    [Required]
    public TimeOnly EndTime { get; set; }
    [Required]
    public TimeOnly TimeInterval { get; set; }
}