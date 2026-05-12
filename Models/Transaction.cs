using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MoneyManage.Api.Models;

public class Transaction
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [StringLength(200)]
    public string Description { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Notes { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }

    [Required]
    public DateTime Date { get; set; } = DateTime.UtcNow;

    public bool IsRecurring { get; set; } = false;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Foreign Keys
    public Guid AccountId { get; set; }
    public Account Account { get; set; } = null!;

    public Guid CategoryId { get; set; }
    public Category Category { get; set; } = null!;
}
