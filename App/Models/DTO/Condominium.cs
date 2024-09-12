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
    public required Guid OwnerId { get; set; }
    [Required]
    public required CondominiumUserRelationshipType RelationshipType { get; set; }
}

public class CondominiumDTO
{

    public Guid Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public required object Owner { get; set; }
    public List<object> Members { get; set; } = [];

    public static CondominiumDTO FromCondominium(Condominium condo)
    {
        return new()
        {
            Id = condo.Id,
            Name = condo.Name,
            Description = condo.Description,
            Owner = new
            {
                Id = condo.Owner.Id,
                FullName = condo.Owner.FullName,
            },
            Members = condo.Members
                .Select<CondominiumUser, object>(member => new
                {
                    User = new
                    {
                        Id = member.User.Id,
                        FullName = member.User.FullName,
                    },
                    RelationshipType = member.RelationshipType switch
                    {
                        CondominiumUserRelationshipType.Tenant => "Tenant",
                        CondominiumUserRelationshipType.Manager => "Manager",
                        _ => throw new HttpResponseException(new ProblemDetails()
                        {
                            Title = "Invalid condominium relationship type",
                            Detail = $"Invalid relationship type on condominium {condo.Id}",
                            Status = StatusCodes.Status500InternalServerError,
                            Type = "https://developer.mozilla.org/en-US/docs/Web/HTTP/Status/500"
                        })
                    }
                }).ToList(),
        };
    }
}