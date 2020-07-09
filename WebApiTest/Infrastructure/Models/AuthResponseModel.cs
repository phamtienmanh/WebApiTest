using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;

namespace WebApiTest.Infrastructure.Models
{
    public class AuthResponseModel
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
        [JsonIgnore] // refresh token is returned in http only cookie
        public string RefreshToken { get; set; }

        public AuthResponseModel(IdentityUser user, string jwtToken, string refreshToken)
        {
            UserName = user.UserName;
            Email = user.Email;
            Token = jwtToken;
            RefreshToken = refreshToken;
        }
    }
}
