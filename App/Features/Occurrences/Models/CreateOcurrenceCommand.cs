using System.ComponentModel.DataAnnotations;

namespace NextCondoApi.Features.OccurrencesFeature.Models;

public class PostOccurrenceCommand
{
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
    public int OccurrenceTypeId { get; set; }
    [Required(AllowEmptyStrings = false)]
    [StringLength(maximumLength: 255)]
    public required string Title { get; set; }
    [StringLength(maximumLength: 4000)]
    public string? Description { get; set; }
}