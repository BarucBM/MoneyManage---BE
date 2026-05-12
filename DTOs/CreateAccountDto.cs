using System.ComponentModel.DataAnnotations;

namespace MoneyManage.Api.DTOs;

public class CreateAccountDto
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }

    [Required]
    public Models.AccountType Type { get; set; }

    [Required]
    public decimal InitialBalance { get; set; }
}
