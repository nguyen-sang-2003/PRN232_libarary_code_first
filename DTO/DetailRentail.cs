namespace LibararyWebApplication.DTO
{
    public class DetailRentail
    {
        public int RentalId { get; set; }
        public int ReturnId { get; set; }
        public int BookCopyId { get; set; }
        public string Title { get; set; }
        public string ImageBase64 { get; set; }

        public DateTime RentailDate { get; set; }
        public DateTime DueDate { get; set; }
        public string BookCondition { get; set; }
        public string Status { get; set; }

    }
}
