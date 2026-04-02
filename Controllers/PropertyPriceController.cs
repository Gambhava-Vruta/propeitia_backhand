using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Propertia.Models;

[ApiController]
[Route("api/[controller]")]
public class PropertyPriceController : ControllerBase
{
    private readonly PropertiaContext _db;

    public PropertyPriceController(PropertiaContext db)
    {
        _db = db;
    }
    [Authorize(Roles = "admin,seller,buyer")]
    [HttpGet("{propertyId}")]
    public async Task<IActionResult> GetByProperty(int propertyId)
    {
        var prices = await _db.PropertyPrices
            .Where(p => p.PropertyId == propertyId)
            .Include(p => p.TransactionType)
            .Select(p => new PropertyPriceDto
            {
                PropertyId = p.PropertyId,
                Amount = p.Amount,
                TransactionType = p.TransactionType.TransactionTypeName
            })
            .ToListAsync();

        return Ok(prices);
    }
    [Authorize(Roles = "admin,seller,buyer")]
    [HttpPost]
    public async Task<IActionResult> Create(PropertyPriceCreateDto dto)
    {
        var propertyExists = await _db.Properties.AnyAsync(p => p.PropertyId == dto.PropertyId);
        var typeExists = await _db.TransactionTypes.AnyAsync(t => t.TransactionTypeId == dto.TransactionTypeId);

        if (!propertyExists || !typeExists)
            return BadRequest(new { message = "Invalid property or transaction type" });

        var price = new PropertyPrice
        {
            PropertyId = dto.PropertyId,
            TransactionTypeId = dto.TransactionTypeId,
            Amount = dto.Amount
        };

        _db.PropertyPrices.Add(price);
        await _db.SaveChangesAsync();

        return Created("", new { price.PropertyId, price.Amount, dto.TransactionTypeId });
    }
}
