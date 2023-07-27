namespace DotnetAPI.Dtos
{
    public partial class UserSalaryDto
    {
        public int UserId { get; set; }
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public decimal Salary { get; set; }



    }
}