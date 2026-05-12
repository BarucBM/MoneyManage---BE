using System.ComponentModel.DataAnnotations;

namespace MoneyManage.Api.DTOs;

public class CreateTransactionDto
{
    [Required]
    [StringLength(200)]
    public string Description { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Notes { get; set; }

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
    public decimal Amount { get; set; }

    [Required]
    public DateTime Date { get; set; } = DateTime.UtcNow;

    public bool IsRecurring { get; set; } = false;

    [Required]
    public Guid AccountId { get; set; }

    [Required]
    public Guid CategoryId { get; set; }
}
