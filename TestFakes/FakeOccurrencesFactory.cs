
using Bogus;
using Bogus.Extensions;

namespace TestFakes;

public class NewOccurrenceDetails : AddOccurrenceDTO { }

public static class FakeOccurrencesFactory
{
    private static Faker<NewOccurrenceDetails> NewOccurrenceDetailsFaker { get; } = new Faker<NewOccurrenceDetails>()
        .RuleFor(o => o.Title, f => f.Name.JobTitle().ClampLength(1, 255))
        .RuleFor(o => o.Description, f => f.Lorem.Paragraph(5).ClampLength(1, 4000))
        .RuleFor(o => o.OccurrenceTypeId, f => f.Random.Int(1, 6));

    public static NewOccurrenceDetails GetFakeNewOccurrenceDetails()
    {
        return NewOccurrenceDetailsFaker.Generate();
    }
}