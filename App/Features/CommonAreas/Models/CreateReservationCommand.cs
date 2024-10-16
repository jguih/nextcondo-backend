
using System.ComponentModel.DataAnnotations;

namespace NextCondoApi.Features.CommonAreasFeature.Models;

public class CreateReservationCommand
{
    [Required]
    public DateOnly Date { get; set; }
    [Required]
    public TimeOnly StartAt { get; set; }
    [Required]
    public int CommonAreaId { get; set; }
}