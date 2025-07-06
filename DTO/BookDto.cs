namespace LibararyWebApplication.DTO
{
    public class BookDto
    {
        public Guid Id { get; set; }
        public Guid? BookSetId { get; set; }
        public string Condition { get; set; }
        public string Status { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string UpdatedBy { get; set; }
        public string? Title { get; set; }
        public decimal? Price { get; set; }
    }
}
