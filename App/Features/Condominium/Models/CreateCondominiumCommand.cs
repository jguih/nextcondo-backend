using System.ComponentModel.DataAnnotations;
using NextCondoApi.Entity;

namespace NextCondoApi.Features.CondominiumFeature.Models;

public class CreateCondominiumCommand
{
    [Required]
    [StringLength(100)]
    public required string Name { get; set; }
    [StringLength(2000)]
    public string? Description { get; set; }
    [Required]
    public required CondominiumUserRelationshipType RelationshipType { get; set; }
}