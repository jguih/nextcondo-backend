
using Bogus;
using Bogus.Extensions;
using NextCondoApi.Features.CommonAreasFeature.Models;

namespace TestFakes;

public class CommonAreaDetails : CreateCommonAreaCommand
{
    public Guid CondominiumId { get; set; }
}

public class FakeCommonAreasFactory
{
    private static readonly Faker<CommonAreaDTO> CommonAreaDTOFaker = new Faker<CommonAreaDTO>()
        .RuleFor(o => o.Id, f => f.Random.Int())
        .RuleFor(o => o.Name, f => f.Lorem.Sentence(3).ClampLength(0, 255))
        .RuleFor(o => o.Description, f => f.Lorem.Paragraph(10).ClampLength(0, 2000))
        .RuleFor(o => o.StartTime, TimeOnly.Parse("00:00"))
        .RuleFor(o => o.EndTime, TimeOnly.Parse("22:00"))
        .RuleFor(o => o.TimeInterval, TimeOnly.Parse("01:00"));

    private static readonly Faker<CommonAreaDetails> CommonAreaDetailsFaker = new Faker<CommonAreaDetails>()
        .RuleFor(o => o.Name, f => f.Lorem.Sentence(3).ClampLength(0, 255))
        .RuleFor(o => o.Description, f => f.Lorem.Paragraph(10).ClampLength(0, 2000))
        .RuleFor(o => o.StartTime, TimeOnly.Parse("00:00"))
        .RuleFor(o => o.EndTime, TimeOnly.Parse("22:00"))
        .RuleFor(o => o.TimeInterval, TimeOnly.Parse("01:00"));

    public static CommonAreaDTO GetDto()
    {
        return CommonAreaDTOFaker.Generate();
    }

    public static CommonAreaDetails GetDetails()
    {
        return CommonAreaDetailsFaker.Generate();
    }
}