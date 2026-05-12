using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoneyManage.Api.Data;
using MoneyManage.Api.DTOs;
using MoneyManage.Api.Models;

namespace MoneyManage.Api.Controllers;

[ApiController]
[Route("accounts")]
public class AccountsController : ControllerBase
{
    private readonly AppDbContext _context;

    public AccountsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AccountResponseDto>>> GetAccounts()
    {
        Console.WriteLine("Fetching accounts...");
        var accounts = await _context.Accounts
            .Select(a => new AccountResponseDto
            {
                Id = a.Id,
                Name = a.Name,
                Description = a.Description,
                Balance = a.Balance,
                Type = a.Type,
                CreatedAt = a.CreatedAt
            })
            .ToListAsync();

        return Ok(accounts);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AccountResponseDto>> GetAccount(Guid id)
    {
        var account = await _context.Accounts.FindAsync(id);

        if (account == null)
        {
            return NotFound();
        }

        return Ok(new AccountResponseDto
        {
            Id = account.Id,
            Name = account.Name,
            Description = account.Description,
            Balance = account.Balance,
            Type = account.Type,
            CreatedAt = account.CreatedAt
        });
    }

    [HttpPost]
    public async Task<ActionResult<AccountResponseDto>> CreateAccount([FromBody] CreateAccountDto dto)
    {
        if (dto == null)
        {
            return BadRequest("Account data is required");
        }

        var account = new Account
        {
            Name = dto.Name,
            Description = dto.Description,
            Type = dto.Type,
            Balance = dto.InitialBalance, // Set initial balance here
            UserId = null, // No longer requires a valid user
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Accounts.Add(account);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetAccount), new { id = account.Id }, new AccountResponseDto
        {
            Id = account.Id,
            Name = account.Name,
            Description = account.Description,
            Balance = account.Balance,
            Type = account.Type,
            CreatedAt = account.CreatedAt
        });
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateAccount(Guid id, [FromBody] CreateAccountDto dto)
    {
        var account = await _context.Accounts.FindAsync(id);

        if (account == null)
        {
            return NotFound();
        }

        account.Name = dto.Name;
        account.Description = dto.Description;
        account.Type = dto.Type;
        account.UpdatedAt = DateTime.UtcNow;

        // We typically don't allow updating the balance directly via PUT
        // because it should be managed via transactions.

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteAccount(Guid id)
    {
        var account = await _context.Accounts.FindAsync(id);

        if (account == null)
        {
            return NotFound();
        }

        _context.Accounts.Remove(account);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
