//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using Propertia.Models;

//[ApiController]
//[Route("api/[controller]")]
//public class AmenitiesController : ControllerBase
//{
//    private readonly PropertiaContext _db;

//    public AmenitiesController(PropertiaContext db)
//    {
//        _db = db;
//    }

//    // =========================
//    // GET: api/amenities
//    // =========================
//    [HttpGet]
//    public async Task<IActionResult> GetAll()
//    {
//        try
//        {
//            var amenities = await _db.Amenities
//                .Select(a => new
//                {
//                    a.AmenityId,
//                    a.AmenityName
//                })
//                .ToListAsync();

//            return Ok(amenities);
//        }
//        catch (Exception ex)
//        {
//            return StatusCode(500, new
//            {
//                message = "Error getting amenities",
//                error = ex.Message
//            });
//        }
//    }

//    // =========================
//    // GET: api/amenities/5
//    // =========================
//    [HttpGet("{id}")]
//    public async Task<IActionResult> GetById(int id)
//    {
//        try
//        {
//            var amenity = await _db.Amenities
//                .Where(a => a.AmenityId == id)
//                .Select(a => new
//                {
//                    a.AmenityId,
//                    a.AmenityName
//                })
//                .FirstOrDefaultAsync();

//            if (amenity == null)
//                return NotFound(new { message = "Amenity not found" });

//            return Ok(amenity);
//        }
//        catch (Exception ex)
//        {
//            return StatusCode(500, new
//            {
//                message = "Error getting amenity",
//                error = ex.Message
//            });
//        }
//    }

//    // =========================
//    // POST: api/amenities
//    // =========================
//    [HttpPost]
//    public async Task<IActionResult> Create(Amenity amenity)
//    {
//        try
//        {
//            if (!ModelState.IsValid)
//                return BadRequest(ModelState);

//            bool exists = await _db.Amenities.AnyAsync(a => a.AmenityName == amenity.AmenityName);
//            if (exists)
//                return BadRequest(new { message = "Amenity already exists" });

//            _db.Amenities.Add(amenity);
//            await _db.SaveChangesAsync();

//            return Created("", new
//            {
//                amenity.AmenityId,
//                amenity.AmenityName
//            });
//        }
//        catch (Exception ex)
//        {
//            return StatusCode(500, new
//            {
//                message = "Error creating amenity",
//                error = ex.Message
//            });
//        }
//    }

//    // =========================
//    // PUT: api/amenities/5
//    // =========================
//    [HttpPut("{id}")]
//    public async Task<IActionResult> Update(int id, Amenity amenity)
//    {
//        try
//        {
//            var existing = await _db.Amenities.FindAsync(id);

//            if (existing == null)
//                return NotFound(new { message = "Amenity not found" });

//            existing.AmenityName = amenity.AmenityName;

//            await _db.SaveChangesAsync();

//            return Ok(new { message = "Amenity updated successfully" });
//        }
//        catch (Exception ex)
//        {
//            return StatusCode(500, new
//            {
//                message = "Error updating amenity",
//                error = ex.Message
//            });
//        }
//    }

//    // =========================
//    // DELETE: api/amenities/5
//    // =========================
//    [HttpDelete("{id}")]
//    public async Task<IActionResult> Delete(int id)
//    {
//        try
//        {
//            var amenity = await _db.Amenities.FindAsync(id);

//            if (amenity == null)
//                return NotFound(new { message = "Amenity not found" });

//            _db.Amenities.Remove(amenity);
//            await _db.SaveChangesAsync();

//            return Ok(new { message = "Amenity deleted successfully" });
//        }
//        catch (Exception ex)
//        {
//            return StatusCode(500, new
//            {
//                message = "Error deleting amenity",
//                error = ex.Message
//            });
//        }
//    }
//}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Propertia.Models;
//using Propertia.DTOs;

[Authorize(Roles = "admin,buyer,seller")]
[ApiController]
[Route("api/[controller]")]
public class AmenitiesController : ControllerBase
{
    private readonly PropertiaContext _db;

    public AmenitiesController(PropertiaContext db)
    {
        _db = db;
    }


    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var amenities = await _db.Amenities
                .Select(a => new AmenityDto
                {
                    AmenityId = a.AmenityId,
                    AmenityName = a.AmenityName
                })
                .ToListAsync();

            return Ok(amenities);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error getting amenities", error = ex.Message });
        }
    }


    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var amenity = await _db.Amenities
                .Where(a => a.AmenityId == id)
                .Select(a => new AmenityDto
                {
                    AmenityId = a.AmenityId,
                    AmenityName = a.AmenityName
                })
                .FirstOrDefaultAsync();

            if (amenity == null)
                return NotFound(new { message = "Amenity not found" });

            return Ok(amenity);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error getting amenity", error = ex.Message });
        }
    }


    [HttpPost]
    public async Task<IActionResult> Create(AmenityDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            bool exists = await _db.Amenities
                .AnyAsync(a => a.AmenityName == dto.AmenityName);

            if (exists)
                return BadRequest(new { message = "Amenity already exists" });

            var amenity = new Amenity
            {
                AmenityName = dto.AmenityName
            };

            _db.Amenities.Add(amenity);
            await _db.SaveChangesAsync();

            return Created("", new AmenityDto
            {
                AmenityId = amenity.AmenityId,
                AmenityName = amenity.AmenityName
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error creating amenity", error = ex.Message });
        }
    }


    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, AmenityDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existing = await _db.Amenities.FindAsync(id);

            if (existing == null)
                return NotFound(new { message = "Amenity not found" });

            existing.AmenityName = dto.AmenityName;
            await _db.SaveChangesAsync();

            return Ok(new { message = "Amenity updated successfully" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error updating amenity", error = ex.Message });
        }
    }


    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var amenity = await _db.Amenities.FindAsync(id);

            if (amenity == null)
                return NotFound(new { message = "Amenity not found" });

            _db.Amenities.Remove(amenity);
            await _db.SaveChangesAsync();

            return Ok(new { message = "Amenity deleted successfully" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error deleting amenity", error = ex.Message });
        }
    }
}
