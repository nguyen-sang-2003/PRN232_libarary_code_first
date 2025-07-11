namespace LibararyWebApplication.DTO.Cuong
{
    public class BookDto
    {
            public int Id { get; set; }
            public string Title { get; set; }
            public string AuthorName { get; set; }
            public string ImageBase64 { get; set; } // Optional: Can convert to full data URL in frontend
            public DateTime PublishedDate { get; set; }
            public int TotalCopies { get; set; }
            public int AvailableCopies { get; set; }

    }
}
