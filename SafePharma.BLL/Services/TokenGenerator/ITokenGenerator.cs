using System.Security.Claims;
using SafePharma.BLL.DTOs;

namespace SafePharma.BLL
{
    public interface ITokenGenerator
    {
        TokenDto GenerateToken(List<Claim> claims);
    }
}