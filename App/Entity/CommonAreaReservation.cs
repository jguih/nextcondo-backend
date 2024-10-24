
namespace NextCondoApi.Entity;

public class CommonAreaReservation : BaseEntity
{
    public int Id { get; set; }
    public DateOnly Date { get; set; }
    public TimeOnly StartAt { get; set; }
    public int CommonAreaId { get; set; }
    public CommonArea? CommonArea { get; set; }
    public int SlotId { get; set; }
    public CommonAreaSlot? Slot { get; set; }
    public Guid UserId { get; set; }
    public User? User { get; set; }

    public override object GetId()
    {
        return Id;
    }
}