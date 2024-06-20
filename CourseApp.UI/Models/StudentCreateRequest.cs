namespace CourseApp.UI.Models
{
    public class StudentCreateRequest
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public DateTime BirthDate { get; set; }
        public int GroupId { get; set; }
        public IFormFile Photo { get; set; }
    }
}
