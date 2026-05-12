using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MoneyManage.Api.Models;

public class Category
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }

    [StringLength(10)]
    public string Color { get; set; } = "#000000";

    [Required]
    public CategoryType Type { get; set; } = CategoryType.Expense;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Foreign Key
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    // Navigation properties
    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}

public enum CategoryType
{
    Income,
    Expense
}
