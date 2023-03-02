using System.ComponentModel.DataAnnotations;

namespace SimpleAPI.Models;

public class HomeViewModel
{
    [Required]
    public string? Question { get; set; }
    public string? Answer { get; set; }
}
