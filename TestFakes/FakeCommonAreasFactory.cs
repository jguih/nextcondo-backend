
using Bogus;
using Bogus.Extensions;
using NextCondoApi.Entity;
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
        .RuleFor(o => o.Type,
            new CommonAreaDTO.CommonAreaTypeDTO()
            {
                Id = 1,
                Name_EN = "Gym",
                Name_PTBR = "Academia"
            })
        .RuleFor(o => o.StartTime, TimeOnly.Parse("00:00"))
        .RuleFor(o => o.EndTime, TimeOnly.Parse("22:00"))
        .RuleFor(o => o.TimeInterval, TimeOnly.Parse("01:00"));

    private static readonly Faker<CommonArea> CommonAreaFaker = new Faker<CommonArea>()
        .RuleFor(o => o.Id, f => f.Random.Int())
        .RuleFor(o => o.TypeId, f => f.Random.Int(1, 6))
        .RuleFor(o => o.StartTime, TimeOnly.Parse("00:00"))
        .RuleFor(o => o.EndTime, TimeOnly.Parse("22:00"))
        .RuleFor(o => o.TimeInterval, TimeOnly.Parse("01:00"))
        .RuleFor(o => o.CondominiumId, f => f.Random.Guid())
        .RuleFor(o => o.CreatedAt, DateTimeOffset.UtcNow)
        .RuleFor(o => o.UpdatedAt, DateTimeOffset.UtcNow);

    private static readonly Faker<CommonAreaDetails> CommonAreaDetailsFaker = new Faker<CommonAreaDetails>()
        .RuleFor(o => o.TypeId, f => f.Random.Int(1, 6))
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

    public static CommonArea Get()
    {
        return CommonAreaFaker.Generate();
    }
}