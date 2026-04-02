using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Propertia.Models;

namespace Propertia.Controllers
{
    [ApiController]
    [Route("api/property-images")]
    public class PropertyImageController : ControllerBase
    {   
        private readonly PropertiaContext _db;

        public PropertyImageController(PropertiaContext db)
        {
            _db = db;
        }

        // =========================
        // POST: api/property-images
        // =========================
        [Authorize(Roles ="admin,seller")]
        [HttpPost]
        public async Task<IActionResult> Create(PropertyImagedto dto)
        {
            bool propertyExists = await _db.Properties
                .AnyAsync(p => p.PropertyId == dto.PropertyId);

            if (!propertyExists)
                return BadRequest(new { message = "Property does not exist" });

            var image = new PropertyImage
            {
                PropertyId = dto.PropertyId,
                ImagePath = dto.ImagePath
            };

            _db.PropertyImages.Add(image);
            await _db.SaveChangesAsync();

            return Ok(new
            {
                image.ImageId,
                image.UploadedAt
            });
        }

        // =========================
        // GET: api/property-images/property/5
        // =========================
        [Authorize(Roles = "admin,seller,buyer")]
        [HttpGet("property/{propertyId}")]
        public async Task<IActionResult> GetByProperty(int propertyId)
        {
            var images = await _db.PropertyImages
                .Where(i => i.PropertyId == propertyId)
                .Select(i => new PropertyImagedto
                {
                    ImageId = i.ImageId,
                    PropertyId = i.PropertyId,
                    ImagePath = i.ImagePath,
                    UploadedAt = i.UploadedAt
                })
                .ToListAsync();

            return Ok(images);
        }

        [Authorize(Roles = "admin,seller")]
        [HttpDelete("by-name/{imageName}")]
        public async Task<IActionResult> DeleteByName(string imageName)
        {
            var image = await _db.PropertyImages
                .FirstOrDefaultAsync(i => i.ImagePath == imageName);

            if (image == null)
                return NotFound(new { message = "Image not found" });

            // Delete file from disk
            string folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
            string filePath = Path.Combine(folder, imageName);
            if (System.IO.File.Exists(filePath))
                System.IO.File.Delete(filePath);

            _db.PropertyImages.Remove(image);
            await _db.SaveChangesAsync();

            return Ok(new { message = "Image deleted successfully" });
        }
    }
}
