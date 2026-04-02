using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Propertia.Models;
[ApiController]
[Route("api/[controller]")]
public class TransactionTypeController : ControllerBase
{
    private readonly PropertiaContext _db;

    public TransactionTypeController(PropertiaContext db)
    {
        _db = db;
    }
    [Authorize(Roles = "admin,buyer,seller")]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var types = await _db.TransactionTypes
            .Select(t => new TransactionTypeDto
            {
                TransactionTypeId = t.TransactionTypeId,
                TransactionTypeName = t.TransactionTypeName
            })
            .ToListAsync();

        return Ok(types);
    }

    [Authorize(Roles = "Admin,buyer,seller")]
    [HttpPost]
    public async Task<IActionResult> Create(TransactionTypeDto dto)
    {
        var type = new TransactionType
        {
            TransactionTypeName = dto.TransactionTypeName
        };

        _db.TransactionTypes.Add(type);
        await _db.SaveChangesAsync();

        return Created("", new { type.TransactionTypeId, type.TransactionTypeName });
    }
}
