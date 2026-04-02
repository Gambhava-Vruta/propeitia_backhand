//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using Propertia.Models;

//[ApiController]
//[Route("api/[controller]")]
//public class BHKController : ControllerBase
//{
//    private readonly PropertiaContext _db;

//    public BHKController(PropertiaContext db)
//    {
//        _db = db;
//    }

//    // =========================
//    // GET: api/bhk
//    // =========================
//    [HttpGet]
//    public async Task<IActionResult> GetAll()
//    {
//        try
//        {
//            var bhks = await _db.BHKs
//                .Include(b => b.Property)
//                .Select(b => new
//                {
//                    b.BHKId,
//                    b.BHKType,
//                    b.TotalWashrooms,
//                    Property = new
//                    {
//                        b.Property.PropertyId,
//                        b.Property.Title
//                    }
//                })
//                .ToListAsync();

//            return Ok(bhks);
//        }
//        catch (Exception ex)
//        {
//            return StatusCode(500, new
//            {
//                message = "Error getting BHKs",
//                error = ex.Message
//            });
//        }
//    }

//    // =========================
//    // GET: api/bhk/5
//    // =========================
//    [HttpGet("{id}")]
//    public async Task<IActionResult> GetById(int id)
//    {
//        try
//        {
//            var bhk = await _db.BHKs
//                .Include(b => b.Property)
//                .Where(b => b.BHKId == id)
//                .Select(b => new
//                {
//                    b.BHKId,
//                    b.BHKType,
//                    b.TotalWashrooms,
//                    Property = new
//                    {
//                        b.Property.PropertyId,
//                        b.Property.Title
//                    }
//                })
//                .FirstOrDefaultAsync();

//            if (bhk == null)
//                return NotFound(new { message = "BHK not found" });

//            return Ok(bhk);
//        }
//        catch (Exception ex)
//        {
//            return StatusCode(500, new
//            {
//                message = "Error getting BHK",
//                error = ex.Message
//            });
//        }
//    }

//    // =========================
//    // POST: api/bhk
//    // =========================
//    [HttpPost]
//    public async Task<IActionResult> Create(BHK bhk)
//    {
//        try
//        {
//            if (!ModelState.IsValid)
//                return BadRequest(ModelState);

//            // Optional: check if Property exists
//            var propertyExists = await _db.Properties.AnyAsync(p => p.PropertyId == bhk.PropertyId);
//            if (!propertyExists)
//                return BadRequest(new { message = "Property does not exist" });

//            _db.BHKs.Add(bhk);
//            await _db.SaveChangesAsync();

//            return Created("", new
//            {
//                bhk.BHKId,
//                bhk.BHKType,
//                bhk.TotalWashrooms,
//                bhk.PropertyId
//            });
//        }
//        catch (Exception ex)
//        {
//            return StatusCode(500, new
//            {
//                message = "Error creating BHK",
//                error = ex.Message
//            });
//        }
//    }

//    // =========================
//    // PUT: api/bhk/5
//    // =========================
//    [HttpPut("{id}")]
//    public async Task<IActionResult> Update(int id, BHK bhk)
//    {
//        try
//        {
//            var existing = await _db.BHKs.FindAsync(id);

//            if (existing == null)
//                return NotFound(new { message = "BHK not found" });

//            existing.BHKType = bhk.BHKType;
//            existing.TotalWashrooms = bhk.TotalWashrooms;

//            // Optional: update PropertyId if needed
//            existing.PropertyId = bhk.PropertyId;

//            await _db.SaveChangesAsync();

//            return Ok(new { message = "BHK updated successfully" });
//        }
//        catch (Exception ex)
//        {
//            return StatusCode(500, new
//            {
//                message = "Error updating BHK",
//                error = ex.Message
//            });
//        }
//    }

//    // =========================
//    // DELETE: api/bhk/5
//    // =========================
//    [HttpDelete("{id}")]
//    public async Task<IActionResult> Delete(int id)
//    {
//        try
//        {
//            var bhk = await _db.BHKs.FindAsync(id);

//            if (bhk == null)
//                return NotFound(new { message = "BHK not found" });

//            _db.BHKs.Remove(bhk);
//            await _db.SaveChangesAsync();

//            return Ok(new { message = "BHK deleted successfully" });
//        }
//        catch (Exception ex)
//        {
//            return StatusCode(500, new
//            {
//                message = "Error deleting BHK",
//                error = ex.Message
//            });
//        }
//    }
//}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Propertia.Models;

[ApiController]
[Route("api/[controller]")]
public class BHKController : ControllerBase
{
    private readonly PropertiaContext _db;

    public BHKController(PropertiaContext db)
    {
        _db = db;
    }

    // =========================
    // GET: api/bhk
    // =========================
    [Authorize(Roles = "admin,buyer,seller")]

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var bhks = await _db.BHKs
                .Include(b => b.Property)
                .Select(b => new BHKdto
                {
                    BHKId = b.BHKId,
                    BHKType = b.BHKType,
                    TotalWashrooms = b.TotalWashrooms,
                    PropertyId = b.PropertyId,
                })
                .ToListAsync();

            return Ok(bhks);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error getting BHKs", error = ex.Message });
        }
    }

    // =========================
    // GET: api/bhk/5
    // =========================
    [Authorize(Roles = "admin,buyer,seller")]

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var bhk = await _db.BHKs
                .Include(b => b.Property)
                .Where(b => b.BHKId == id)
                .Select(b => new BHKdto
                {
                    BHKId = b.BHKId,
                    BHKType = b.BHKType,
                    TotalWashrooms = b.TotalWashrooms,
                    PropertyId = b.PropertyId,
                })
                .FirstOrDefaultAsync();

            if (bhk == null)
                return NotFound(new { message = "BHK not found" });

            return Ok(bhk);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error getting BHK", error = ex.Message });
        }
    }

    // =========================
    // POST: api/bhk
    // =========================
    [Authorize(Roles = "admin,seller")]

    [HttpPost]
    public async Task<IActionResult> Create(BHKdto dto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Check Property existence
            bool propertyExists = await _db.Properties
                .AnyAsync(p => p.PropertyId == dto.PropertyId);

            if (!propertyExists)
                return BadRequest(new { message = "Property does not exist" });

            var bhk = new BHK
            {
                PropertyId = dto.PropertyId,
                BHKType = dto.BHKType,
                TotalWashrooms = dto.TotalWashrooms
            };

            _db.BHKs.Add(bhk);
            await _db.SaveChangesAsync();

            return Created("", new { bhk.BHKId });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error creating BHK", error = ex.Message });
        }
    }

    // =========================
    // PUT: api/bhk/5
    // =========================
    [Authorize(Roles = "admin,seller")]

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, BHKdto dto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existing = await _db.BHKs.FindAsync(id);
            if (existing == null)
                return NotFound(new { message = "BHK not found" });

            bool propertyExists = await _db.Properties
                .AnyAsync(p => p.PropertyId == dto.PropertyId);

            if (!propertyExists)
                return BadRequest(new { message = "Property does not exist" });

            existing.PropertyId = dto.PropertyId;
            existing.BHKType = dto.BHKType;
            existing.TotalWashrooms = dto.TotalWashrooms;

            await _db.SaveChangesAsync();

            return Ok(new { message = "BHK updated successfully" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error updating BHK", error = ex.Message });
        }
    }

    // =========================
    // DELETE: api/bhk/5
    // =========================
    [Authorize(Roles = "admin,seller")]

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var bhk = await _db.BHKs.FindAsync(id);

            if (bhk == null)
                return NotFound(new { message = "BHK not found" });

            _db.BHKs.Remove(bhk);
            await _db.SaveChangesAsync();

            return Ok(new { message = "BHK deleted successfully" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error deleting BHK", error = ex.Message });
        }
    }
}
