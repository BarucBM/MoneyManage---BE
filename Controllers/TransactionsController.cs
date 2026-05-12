using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoneyManage.Api.Data;
using MoneyManage.Api.DTOs;
using MoneyManage.Api.Models;

namespace MoneyManage.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TransactionsController : ControllerBase
{
    private readonly AppDbContext _context;

    public TransactionsController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/transactions
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TransactionResponseDto>>> GetAll(
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate,
        [FromQuery] Guid? accountId,
        [FromQuery] Guid? categoryId)
    {
        var query = _context.Transactions
            .Include(t => t.Account)
            .Include(t => t.Category)
            .AsQueryable();

        if (startDate.HasValue)
            query = query.Where(t => t.Date >= startDate.Value);

        if (endDate.HasValue)
            query = query.Where(t => t.Date <= endDate.Value);

        if (accountId.HasValue)
            query = query.Where(t => t.AccountId == accountId.Value);

        if (categoryId.HasValue)
            query = query.Where(t => t.CategoryId == categoryId.Value);

        var transactions = await query
            .OrderByDescending(t => t.Date)
            .ToListAsync();

        var response = transactions.Select(t => new TransactionResponseDto
        {
            Id = t.Id,
            Description = t.Description,
            Notes = t.Notes,
            Amount = t.Amount,
            Date = t.Date,
            IsRecurring = t.IsRecurring,
            CreatedAt = t.CreatedAt,
            AccountId = t.AccountId,
            AccountName = t.Account?.Name ?? string.Empty,
            CategoryId = t.CategoryId,
            CategoryName = t.Category?.Name ?? string.Empty,
            CategoryColor = t.Category?.Color ?? string.Empty
        });

        return Ok(response);
    }

    // GET: api/transactions/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<TransactionResponseDto>> GetById(Guid id)
    {
        var transaction = await _context.Transactions
            .Include(t => t.Account)
            .Include(t => t.Category)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (transaction == null)
            return NotFound(new { message = "Transaction not found" });

        var response = new TransactionResponseDto
        {
            Id = transaction.Id,
            Description = transaction.Description,
            Notes = transaction.Notes,
            Amount = transaction.Amount,
            Date = transaction.Date,
            IsRecurring = transaction.IsRecurring,
            CreatedAt = transaction.CreatedAt,
            AccountId = transaction.AccountId,
            AccountName = transaction.Account?.Name ?? string.Empty,
            CategoryId = transaction.CategoryId,
            CategoryName = transaction.Category?.Name ?? string.Empty,
            CategoryColor = transaction.Category?.Color ?? string.Empty
        };

        return Ok(response);
    }

    // POST: api/transactions
    [HttpPost]
    public async Task<ActionResult<TransactionResponseDto>> Create([FromBody] CreateTransactionDto dto)
    {
        // Validate Account exists
        var account = await _context.Accounts.FindAsync(dto.AccountId);
        if (account == null)
            return BadRequest(new { message = "Account not found" });

        // Validate Category exists
        var category = await _context.Categories.FindAsync(dto.CategoryId);
        if (category == null)
            return BadRequest(new { message = "Category not found" });

        var transaction = new Transaction
        {
            Id = Guid.NewGuid(),
            Description = dto.Description,
            Notes = dto.Notes,
            Amount = dto.Amount,
            Date = dto.Date,
            IsRecurring = dto.IsRecurring,
            AccountId = dto.AccountId,
            CategoryId = dto.CategoryId,
            CreatedAt = DateTime.UtcNow
        };

        // Update account balance
        if (category.Type == CategoryType.Income)
            account.Balance += dto.Amount;
        else
            account.Balance -= dto.Amount;

        _context.Transactions.Add(transaction);
        await _context.SaveChangesAsync();

        // Reload with related data
        await _context.Entry(transaction)
            .Reference(t => t.Account)
            .LoadAsync();
        await _context.Entry(transaction)
            .Reference(t => t.Category)
            .LoadAsync();

        var response = new TransactionResponseDto
        {
            Id = transaction.Id,
            Description = transaction.Description,
            Notes = transaction.Notes,
            Amount = transaction.Amount,
            Date = transaction.Date,
            IsRecurring = transaction.IsRecurring,
            CreatedAt = transaction.CreatedAt,
            AccountId = transaction.AccountId,
            AccountName = transaction.Account?.Name ?? string.Empty,
            CategoryId = transaction.CategoryId,
            CategoryName = transaction.Category?.Name ?? string.Empty,
            CategoryColor = transaction.Category?.Color ?? string.Empty
        };

        return CreatedAtAction(nameof(GetById), new { id = transaction.Id }, response);
    }

    // PUT: api/transactions/{id}
    [HttpPut("{id}")]
    public async Task<ActionResult<TransactionResponseDto>> Update(Guid id, [FromBody] UpdateTransactionDto dto)
    {
        var transaction = await _context.Transactions
            .Include(t => t.Category)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (transaction == null)
            return NotFound(new { message = "Transaction not found" });

        // Validate Account exists
        var account = await _context.Accounts.FindAsync(dto.AccountId);
        if (account == null)
            return BadRequest(new { message = "Account not found" });

        // Validate Category exists
        var newCategory = await _context.Categories.FindAsync(dto.CategoryId);
        if (newCategory == null)
            return BadRequest(new { message = "Category not found" });

        // Revert old balance impact
        if (transaction.Category?.Type == CategoryType.Income)
            account.Balance -= transaction.Amount;
        else
            account.Balance += transaction.Amount;

        // Update transaction
        transaction.Description = dto.Description;
        transaction.Notes = dto.Notes;
        transaction.Amount = dto.Amount;
        transaction.Date = dto.Date;
        transaction.IsRecurring = dto.IsRecurring;
        transaction.AccountId = dto.AccountId;
        transaction.CategoryId = dto.CategoryId;

        // Apply new balance impact
        if (newCategory.Type == CategoryType.Income)
            account.Balance += dto.Amount;
        else
            account.Balance -= dto.Amount;

        await _context.SaveChangesAsync();

        // Reload with related data
        await _context.Entry(transaction)
            .Reference(t => t.Account)
            .LoadAsync();
        await _context.Entry(transaction)
            .Reference(t => t.Category)
            .LoadAsync();

        var response = new TransactionResponseDto
        {
            Id = transaction.Id,
            Description = transaction.Description,
            Notes = transaction.Notes,
            Amount = transaction.Amount,
            Date = transaction.Date,
            IsRecurring = transaction.IsRecurring,
            CreatedAt = transaction.CreatedAt,
            AccountId = transaction.AccountId,
            AccountName = transaction.Account?.Name ?? string.Empty,
            CategoryId = transaction.CategoryId,
            CategoryName = transaction.Category?.Name ?? string.Empty,
            CategoryColor = transaction.Category?.Color ?? string.Empty
        };

        return Ok(response);
    }

    // DELETE: api/transactions/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var transaction = await _context.Transactions
            .Include(t => t.Category)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (transaction == null)
            return NotFound(new { message = "Transaction not found" });

        // Revert balance impact before deleting
        var account = await _context.Accounts.FindAsync(transaction.AccountId);
        if (account != null)
        {
            if (transaction.Category?.Type == CategoryType.Income)
                account.Balance -= transaction.Amount;
            else
                account.Balance += transaction.Amount;
        }

        _context.Transactions.Remove(transaction);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // GET: api/transactions/summary
    [HttpGet("summary")]
    public async Task<ActionResult<object>> GetSummary(
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate)
    {
        var query = _context.Transactions
            .Include(t => t.Category)
            .AsQueryable();

        if (startDate.HasValue)
            query = query.Where(t => t.Date >= startDate.Value);

        if (endDate.HasValue)
            query = query.Where(t => t.Date <= endDate.Value);

        var transactions = await query.ToListAsync();

        var income = transactions
            .Where(t => t.Category?.Type == CategoryType.Income)
            .Sum(t => t.Amount);

        var expenses = transactions
            .Where(t => t.Category?.Type == CategoryType.Expense)
            .Sum(t => t.Amount);

        return Ok(new
        {
            TotalIncome = income,
            TotalExpenses = expenses,
            Balance = income - expenses,
            TransactionCount = transactions.Count
        });
    }
}
