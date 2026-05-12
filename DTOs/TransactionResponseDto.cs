namespace MoneyManage.Api.DTOs;

public class TransactionResponseDto
{
    public Guid Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public bool IsRecurring { get; set; }
    public DateTime CreatedAt { get; set; }

    // Related data
    public Guid AccountId { get; set; }
    public string AccountName { get; set; } = string.Empty;

    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string CategoryColor { get; set; } = string.Empty;
}
