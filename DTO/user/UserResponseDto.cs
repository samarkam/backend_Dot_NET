namespace backend.DTO.user
{
    public class UserResponseDto
    {

        public int UserId { get; set; }
        public required string UserName { get; set; }
        public required string Email { get; set; }
    }
}
