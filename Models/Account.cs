using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MoneyManage.Api.Models;

public class Account
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Balance { get; set; } = 0;

    [Required]
    public AccountType Type { get; set; } = AccountType.Checking;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Foreign Key (Optional for personal use)
    public Guid? UserId { get; set; }
    public User? User { get; set; }

    // Navigation properties
    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}

public enum AccountType
{
    Checking,
    Savings,
    CreditCard,
    Cash,
    Investment,
    Other
}
