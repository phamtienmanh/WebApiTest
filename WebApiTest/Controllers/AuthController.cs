using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApiTest.Infrastructure.Extensions;
using WebApiTest.Infrastructure.Models;
using WebApiTest.Infrastructure.Services;

namespace WebApiTest.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController: ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IAuthServices _authServices;

        public AuthController(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            IAuthServices authServices)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _authServices = authServices;
        }

        [HttpGet("google-login")]
        public IActionResult LoginGoogle()
        {
            string redirectUrl = Url.Action("GoogleResponse", "Auth");
            var properties = _signInManager.ConfigureExternalAuthenticationProperties("Google", redirectUrl);
            return new ChallengeResult("Google", properties);
        }

        [HttpGet]
        public async Task<IActionResult> GoogleResponse()
        {
            ExternalLoginInfo info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
                return Unauthorized();

            IdentityUser user;
            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, false);
            
            if (result.Succeeded)
            {
                var userEmail = info.Principal.FindFirst(ClaimTypes.Email).Value;
                user = await _authServices.GetUser(userEmail);
                var authResponse = await _authServices.Authenticate(user, IpAddress());
                return Ok(authResponse);
            }

            user = new IdentityUser
            {
                Email = info.Principal.FindFirst(ClaimTypes.Email).Value,
                UserName = info.Principal.FindFirst(ClaimTypes.Email).Value
            };

            var identResult = await _userManager.CreateAsync(user);
            if (identResult.Succeeded)
            {
                identResult = await _userManager.AddLoginAsync(user, info);
                if (identResult.Succeeded)
                {
                    var authResponse = await _authServices.Authenticate(user, IpAddress());
                    return Ok(authResponse);
                }
            }
            return Unauthorized();
        }

        [HttpPost("registration")]
        public async Task<IActionResult> Create([FromBody]RegistrationModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.ToDictionary());
            }

            var result = await _authServices.Registration(model);

            if (!result.Succeeded && result.Errors.Any())
            {
                foreach (var err in result.Errors)
                {
                    ModelState.AddModelError(err.Code, err.Description);
                }

                return BadRequest(ModelState.ToDictionary());
            }

            return Ok();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody]RegistrationModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.ToDictionary());
            }

            var user = await _authServices.GetUser(model.UserName, model.Password);
            if (user == null)
            {
                ModelState.AddModelError("login_failure", "Invalid username or password.");
                return BadRequest(ModelState.ToDictionary());
            }
            var authResponse = await _authServices.Authenticate(user, IpAddress());
            if (authResponse == null)
            {
                ModelState.AddModelError("login_failure", "Invalid username or password.");
                return BadRequest(ModelState.ToDictionary());
            }
            return Ok(authResponse);
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RevokeTokenModel model)
        {
            var refreshToken = model.Token ?? Request.Cookies["refreshToken"];
            var response = await _authServices.RefreshToken(refreshToken, IpAddress());

            if (response == null)
                return Unauthorized(new { message = "Invalid token" });

            SetTokenCookie(response.RefreshToken);

            return Ok(response);
        }

        [HttpPost("revoke-token")]
        public IActionResult RevokeToken([FromBody] RevokeTokenModel model)
        {
            // accept token from request body or cookie
            var token = model.Token ?? Request.Cookies["refreshToken"];

            if (string.IsNullOrEmpty(token))
                return BadRequest(new { message = "Token is required" });

            var response = _authServices.RevokeToken(token, IpAddress());

            if (!response)
                return NotFound(new { message = "Token not found" });

            return Ok(new { message = "Token revoked" });
        }

        private void SetTokenCookie(string token)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddDays(7)
            };
            Response.Cookies.Append("refreshToken", token, cookieOptions);
        }

        private string IpAddress()
        {
            if (Request.Headers.ContainsKey("X-Forwarded-For"))
                return Request.Headers["X-Forwarded-For"];
            return HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
        }
    }
}
