namespace SafePharma.BLL.DTOs
{
    public record TokenDto(
     string AccessToken,
     int DurationInMinutes,
     string TokenType = "Bearer"
 );

}
