using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Propertia.Models;

namespace Propertia.Controllers
{
    [ApiController]
    [Route("api/property-inquiries")]
    public class PropertyInquiryController : ControllerBase
    {
        private readonly PropertiaContext _db;

        public PropertyInquiryController(PropertiaContext db)
        {
            _db = db;
        }

        [Authorize(Roles = "admin,buyer,seller")]
        [HttpPost]
        public async Task<IActionResult> Create(PropertyInquiryDto dto)
        {
            bool propertyExists = await _db.Properties.AnyAsync(p => p.PropertyId == dto.PropertyId);
            bool userExists = await _db.Users.AnyAsync(u => u.UserId == dto.UserId);

            if (!propertyExists || !userExists)
                return BadRequest(new { message = "Invalid Property or User" });

            var inquiry = new PropertyInquiry
            {
                PropertyId = dto.PropertyId,
                UserId = dto.UserId,
                Message = dto.Message
            };

            _db.PropertyInquiries.Add(inquiry);
            await _db.SaveChangesAsync();

            return Ok(new { inquiry.InquiryId, inquiry.InquiryDate });
        }

        [Authorize(Roles = "admin,buyer,seller")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var inquiries = await _db.PropertyInquiries
                .Include(i => i.Property)
                .Include(i => i.User)
                .Select(i => new
                {
                    i.InquiryId,
                    i.PropertyId,
                    PropertyTitle = i.Property.Title,
                    i.UserId,
                    UserName = i.User.Name,
                    i.Message,
                    i.InquiryDate,
                    i.OwnerReply,
                    i.ReplyDate
                })
                .ToListAsync();

            return Ok(inquiries);
        }

        [Authorize(Roles = "admin,buyer,seller")]
        [HttpGet("property/{propertyId}")]
        public async Task<IActionResult> GetByPropertyId(int propertyId)
        {
            var inquiries = await _db.PropertyInquiries
                .Include(i => i.User)
                .Where(i => i.PropertyId == propertyId)
                .Select(i => new
                {
                    inquiryId = i.InquiryId,
                    propertyId = i.PropertyId,
                    user = new
                    {
                        userId = i.User.UserId,
                        name = i.User.Name
                    },
                    query = i.Message,
                    answer = i.OwnerReply,
                    createdAt = i.InquiryDate,
                    replyDate = i.ReplyDate
                })
                .OrderByDescending(i => i.createdAt)
                .ToListAsync();

            // IMPORTANT: return empty list, NOT 404
            return Ok(inquiries);
        }



        [Authorize(Roles = "seller,admin")]
        [HttpPut("reply/{inquiryId}")]
        public async Task<IActionResult> ReplyToInquiry(
    int inquiryId,
    [FromBody] string reply
)
        {
            var inquiry = await _db.PropertyInquiries.FindAsync(inquiryId);
            if (inquiry == null)
                return NotFound(new { message = "Inquiry not found" });

            inquiry.OwnerReply = reply;
            inquiry.ReplyDate = DateTime.Now;

            await _db.SaveChangesAsync();

            return Ok(new
            {
                message = "Reply sent by seller",
                repliedBy = User.Identity?.Name
            });
        }


    }
}
