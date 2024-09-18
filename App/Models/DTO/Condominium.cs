using Microsoft.AspNetCore.Mvc;
using NextCondoApi.Entity;
using NextCondoApi.Features.Validation;
using System.ComponentModel.DataAnnotations;

namespace NextCondoApi.Models.DTO;

public class AddCondominiumDTO
{
    [Required]
    [StringLength(100)]
    public required string Name { get; set; }
    [StringLength(2000)]
    public string? Description { get; set; }
    [Required]
    public required CondominiumUserRelationshipType RelationshipType { get; set; }
}

public class CondominiumDTO
{
    public class CondominiumOwnerDTO
    {
        public required Guid Id { get; set; }
        public required string FullName { get; set; }
    }

    public class CondominiumMemberDTO 
    {
        public required Guid Id { get; set; }
        public required string FullName { get; set; }
        public required string RelationshipType { get; set; }
    }

    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public required CondominiumOwnerDTO Owner { get; set; }
    public IEnumerable<CondominiumMemberDTO> Members { get; set; } = [];
}