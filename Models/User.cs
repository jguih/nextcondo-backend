
using System.ComponentModel.DataAnnotations;

namespace SimplifyCondoApi.Model;

public class User
{
  [Key]
  public Guid Id { get; set; }
  public required string Email { get; set; }
}