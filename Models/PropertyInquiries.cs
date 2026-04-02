using System.ComponentModel.DataAnnotations;

namespace Propertia.Models
{
    public class PropertyInquiry
    {
        [Key]
        public int InquiryId { get; set; }

        public int PropertyId { get; set; }
        public Property Property { get; set; } = null!;

        public int UserId { get; set; }   // Inquiry sender
        public User User { get; set; } = null!;

        public string? Message { get; set; }
        public DateTime InquiryDate { get; set; } = DateTime.Now;

        public string? OwnerReply { get; set; }
        public DateTime? ReplyDate { get; set; }

        public bool IsReplied => !string.IsNullOrEmpty(OwnerReply);
    }

    public class PropertyInquiryDto
    {
        public int InquiryId { get; set; }
        public int PropertyId { get; set; }
        public int UserId { get; set; }

        public string? Message { get; set; }
        public DateTime InquiryDate { get; set; }

        //public string? OwnerReply { get; set; }
        //public DateTime? ReplyDate { get; set; }
    }

}