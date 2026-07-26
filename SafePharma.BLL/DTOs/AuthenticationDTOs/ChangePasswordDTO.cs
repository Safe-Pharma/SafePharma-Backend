namespace SafePharma.BLL.DTOs
{
    public class ChangePasswordDTO
    {
        required
        public string CurrentPassword { get; set; }

        required
        public string NewPassword { get; set; }
    }

}
