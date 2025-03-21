using System.ComponentModel.DataAnnotations;
namespace SimpleAPI.DTO;

public class CustomerDto
{
  [Required]
  [StringLength(50)]
  public string Name { get; set; } = default!;
  [Required]
  [StringLength(50)]
  public string Surname { get; set; } = default!;
}
