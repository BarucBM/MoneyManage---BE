namespace MoneyManage.Api.DTOs;

public class AccountResponseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Balance { get; set; }
    public Models.AccountType Type { get; set; }
    public DateTime CreatedAt { get; set; }
}
