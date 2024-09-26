
using System.ComponentModel.DataAnnotations;

public class OccurrenceTypeDTO
{
    public required int Id { get; set; }
    public required string Name_EN { get; set; }
    public required string Name_PTBR { get; set; }
    public string? Description_EN { get; set; }
    public string? Description_PTBR { get; set; }
}

public class AddOccurrenceDTO
{
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
    public int OccurrenceTypeId { get; set; }
    [Required(AllowEmptyStrings = false)]
    [StringLength(maximumLength: 255)]
    public required string Title { get; set; }
    [StringLength(maximumLength: 4000)]
    public string? Description { get; set; }
}

public class OccurrenceDTO
{
    public class OccurrenceDTOCreator
    {
        public required Guid Id { get; set; }
        public required string FullName { get; set; }
    }
    public class OccurrenceDTOCondominium
    {
        public required Guid Id { get; set; }
    }
    public class OccurrenceDTOType
    {
        public required int Id { get; set; }
        public required string Name_EN { get; set; }
        public required string Name_PTBR { get; set; }
    }
    public required Guid Id { get; set; }
    public required OccurrenceDTOCreator Creator { get; set; }
    public required OccurrenceDTOCondominium Condominium { get; set; }
    public required OccurrenceDTOType OccurrenceType { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public DateTimeOffset? CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}