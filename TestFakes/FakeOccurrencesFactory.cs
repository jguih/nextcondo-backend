
using Bogus;
using Bogus.Extensions;
using NextCondoApi.Entity;
using NextCondoApi.Features.OccurrencesFeature.Models;

namespace TestFakes;

public class NewOccurrenceDetails : PostOccurrenceCommand { }

public static class FakeOccurrencesFactory
{
    private static Faker<NewOccurrenceDetails> NewOccurrenceDetailsFaker { get; } = new Faker<NewOccurrenceDetails>()
        .RuleFor(o => o.Title, f => f.Name.JobTitle().ClampLength(1, 255))
        .RuleFor(o => o.Description, f => f.Lorem.Paragraph(5).ClampLength(1, 4000))
        .RuleFor(o => o.OccurrenceTypeId, f => f.Random.Int(1, 6));

    private static Faker<Occurrence> OccurrenceFaker { get; } = new Faker<Occurrence>()
        .RuleFor(o => o.Id, f => f.Random.Guid())
        .RuleFor(o => o.Title, f => f.Lorem.Sentence().ClampLength(0, 255))
        .RuleFor(o => o.Description, f => f.Lorem.Paragraph().ClampLength(0, 4000))
        .RuleFor(o => o.OccurrenceTypeId, f => f.Random.Int(1, 6));

    public static NewOccurrenceDetails GetFakeNewOccurrenceDetails()
    {
        return NewOccurrenceDetailsFaker.Generate();
    }

    public static Occurrence GetFakeOccurrence()
    {
        return OccurrenceFaker.Generate();
    }

    public static List<Occurrence> GetFakeOccurrenceListBetween(int min, int max)
    {
        return OccurrenceFaker.GenerateBetween(min, max);
    }
}