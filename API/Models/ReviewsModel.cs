namespace ModelService
{
    public class ReviewsModel
    {
        public int? ProductId { get; set; }
        public decimal AverageRating { get; set; }
        public List<ReviewInfoModel>? Reviews { get; set; }
    }

    public class ReviewInfoModel
    {
        public int ReviewId { get; set; }
        public int? CustomerId { get; set; }
        public string? UserName { get; set; }
        public int? Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime? CreatedAt { get; set; }
    }

    public class ReviewInsertModel
    {
        public int CustomerId { get; set; }
        public int ProductId { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
    }
}