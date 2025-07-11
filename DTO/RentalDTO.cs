using System.Runtime.CompilerServices;

namespace LibararyWebApplication.DTO
{
    public class RentalDTO
    {
        public int Id { get; set; }
        public int BookCopyId { get; set; }
        public string Status { get; set; }
        public DateTime RentalDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public String BookName { get; set; }
        public string UserName { get; set; }

    }
}
