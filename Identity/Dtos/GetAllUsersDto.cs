using System;
namespace Identity.Dtos
{
    public class GetAllUsersDto
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string LockoutDate { get; set; }
        public string UserRole { get; set; }
    }
}