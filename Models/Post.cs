namespace DotnetAPI.Models
{
    public partial class Post
    {
        public int PostId { get; set; }
        public int UserId { get; set; }
        public string PostTitle { get; set; } = "";
        public string PostContent { get; set; } = "";
        public DateTime PostCreated { get; set; } = DateTime.Now;
        public DateTime PostUpdated { get; set; } = DateTime.Now;

    }
}