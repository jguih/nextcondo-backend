
using System.ComponentModel.DataAnnotations;
using NextCondoApi.Entity;

namespace NextCondoApi.Features.CommonAreasFeature.Models;

public class CreateCommonAreaCommandSlot
{
    [Required(AllowEmptyStrings = false)]
    public required string Name_EN { get; set; }
    [Required(AllowEmptyStrings = false)]
    public required string Name_PTBR { get; set; }
}

public class CreateCommonAreaCommand
{

    [Required]
    public required int TypeId { get; set; }
    [Required]
    public required int TimezoneOffsetMinutes { get; set; }
    [Required]
    public TimeOnly StartTime { get; set; }
    [Required]
    public TimeOnly EndTime { get; set; }
    [Required]
    public TimeOnly TimeInterval { get; set; }
    [Required]
    public required List<CreateCommonAreaCommandSlot> Slots { get; set; }

    public List<CommonAreaSlot> GetSlots()
    {
        List<CommonAreaSlot> slots = (from slot in Slots
                                      select new CommonAreaSlot()
                                      {
                                          Name_EN = slot.Name_EN,
                                          Name_PTBR = slot.Name_PTBR
                                      }).ToList();
        return slots;
    }
}