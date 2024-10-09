
using System.ComponentModel.DataAnnotations;
using NextCondoApi.Entity;

namespace NextCondoApi.Features.CondominiumFeature.Models;

public class JoinCommand
{
    [Required]
    public Guid CondominiumId { get; set; }
    [Required]
    public CondominiumUserRelationshipType RelationshipType { get; set; }
}