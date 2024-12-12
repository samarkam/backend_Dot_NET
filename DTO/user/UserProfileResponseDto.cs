namespace backend.DTO.user
{
    public class UserProfileResponseDto
    {

        public int UserProfileId { get; set; }
        public string? Name { get; set; }
        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }
        public int UserId { get; set; }
        public UserResponseDto? User { get; set; }
    }
}
