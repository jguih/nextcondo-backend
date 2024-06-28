
using System.ComponentModel.DataAnnotations;

namespace SimplifyCondoApi.Model;

public class Role
{
  [Key]
  public required string Name { get; set; }
}