using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Propertia.Models;

[ApiController]
[Route("api/[controller]")]
public class PropertyAddressController : ControllerBase
{
    private readonly PropertiaContext _db;

    public PropertyAddressController(PropertiaContext db)
    {
        _db = db;
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var addresses = await _db.PropertyAddresses
                .Select(a => new PropertyAddressdto
                {
                    PropertyAddressId = a.PropertyAddressId,
                    Location = a.Location,
                    City = a.City,
                    State = a.State,
                    Country = a.Country,
                    SocietyName =a.SocietyName,
                    Landmark =a.Landmark,
                    FamousArea=a.FamousArea,
                    Latitude = a.Latitude,
                    Longitude = a.Longitude
                })
                .ToListAsync();

            return Ok(addresses);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                message = "Error getting addresses",
                error = ex.Message
            });
        }
    }

    [Authorize]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var address = await _db.PropertyAddresses
                .Where(a => a.PropertyAddressId == id)
                .Select(a => new PropertyAddressdto
                {
                    PropertyAddressId = a.PropertyAddressId,
                    Location = a.Location,
                    City = a.City,
                    State = a.State,
                    Country = a.Country,
                    SocietyName = a.SocietyName,
                    Landmark = a.Landmark,
                    FamousArea = a.FamousArea,
                    Latitude = a.Latitude,
                    Longitude = a.Longitude

                })
                .FirstOrDefaultAsync();

            if (address == null)
                return NotFound(new { message = "Property address not found" });

            return Ok(address);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                message = "Error getting property address",
                error = ex.Message
            });
        }
    }

    [Authorize(Roles ="seller,admin")]
    [HttpPost]
    public async Task<IActionResult> Create(PropertyAddressdto dto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var address = new PropertyAddress
            {
                Location = dto.Location,
                City = dto.City,
                State = dto.State,
                Country = dto.Country,
                SocietyName = dto.SocietyName,
                Landmark = dto.Landmark,
                FamousArea = dto.FamousArea,
                Latitude = dto.Latitude,
                Longitude = dto.Longitude
            };

            _db.PropertyAddresses.Add(address);
            await _db.SaveChangesAsync();

            return Created("", new
            {
                address.PropertyAddressId,
                address.Location,
                address.City,
                address.SocietyName,
                address.Landmark,
                address.FamousArea,
                address.Latitude,
                address.Longitude
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                message = "Error creating property address",
                error = ex.Message
            });
        }
    }

    [Authorize(Roles = "seller,admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, PropertyAddressdto dto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existing = await _db.PropertyAddresses.FindAsync(id);
            if (existing == null)
                return NotFound(new { message = "Property address not found" });

            existing.Location = dto.Location;
            existing.City = dto.City;
            existing.State = dto.State;
            existing.Country = dto.Country;
            existing.Country = dto.Country;
            existing.SocietyName = dto.SocietyName;
            existing.Landmark = dto.Landmark;
            existing.FamousArea = dto.FamousArea;
            existing.Latitude = dto.Latitude;
            existing.Longitude = dto.Longitude;

            await _db.SaveChangesAsync();

            return Ok(new { message = "Property address updated successfully" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                message = "Error updating property address",
                error = ex.Message
            });
        }
    }

    [Authorize(Roles = "seller,admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var address = await _db.PropertyAddresses.FindAsync(id);
            if (address == null)
                return NotFound(new { message = "Property address not found" });

            // Optional safety check
            bool inUse = await _db.Properties
                .AnyAsync(p => p.PropertyAddressId == id);

            if (inUse)
                return BadRequest(new
                {
                    message = "Cannot delete address because it is linked to properties"
                });

            _db.PropertyAddresses.Remove(address);
            await _db.SaveChangesAsync();

            return Ok(new { message = "Property address deleted successfully" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                message = "Error deleting property address",
                error = ex.Message
            });
        }
    }
}
