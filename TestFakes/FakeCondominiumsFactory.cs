
using Bogus;
using Bogus.Extensions;
using NextCondoApi.Entity;
using NextCondoApi.Features.CondominiumFeature.Models;

namespace TestFakes;

public class NewCondominiumDetails : CreateCondominiumCommand { }

public class FakeCondominiumsFactory
{
    private static Faker<Condominium> CondominiumFaker { get; } = new Faker<Condominium>()
        .RuleFor(o => o.Id, f => f.Random.Guid())
        .RuleFor(o => o.Name, f => f.Lorem.Sentence(10).ClampLength(1, 100))
        .RuleFor(o => o.Description, f => f.Lorem.Paragraph(3).ClampLength(1, 2000))
        .RuleFor(o => o.OwnerId, f => f.Random.Guid())
        .RuleFor(o => o.CreatedAt, DateTimeOffset.UtcNow)
        .RuleFor(o => o.UpdatedAt, DateTimeOffset.UtcNow);

    private static Faker<NewCondominiumDetails> NewCondominiumDetailsFaker { get; } = new Faker<NewCondominiumDetails>()
        .RuleFor(o => o.Name, f => f.Lorem.Sentence(10).ClampLength(1, 100))
        .RuleFor(o => o.Description, f => f.Lorem.Paragraph(3).ClampLength(1, 2000))
        .RuleFor(o => o.RelationshipType, CondominiumUserRelationshipType.Tenant);

    public static Condominium GetFakeCondominium()
    {
        return CondominiumFaker.Generate();
    }

    public static NewCondominiumDetails GetFakeNewCondominiumDetails()
    {
        return NewCondominiumDetailsFaker.Generate();
    }
}
