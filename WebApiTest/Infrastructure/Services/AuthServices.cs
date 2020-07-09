using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using WebApiTest.DataAccess.Contexts;
using WebApiTest.DataAccess.Entities;
using WebApiTest.Infrastructure.Models;

namespace WebApiTest.Infrastructure.Services
{
    public interface IAuthServices
    {
        Task<IdentityResult> Registration(RegistrationModel model);
        Task<IdentityUser> GetUser(string userName, string password = null);
        Task<AuthResponseModel> Authenticate(IdentityUser user, string ipAddress);
        Task<AuthResponseModel> RefreshToken(string token, string ipAddress);
        bool RevokeToken(string token, string ipAddress);
    }

    public class AuthServices : IAuthServices
    {
        private readonly AppDbContext _dbContext;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly UserManager<IdentityUser> _userManager;

        public AuthServices(AppDbContext dbContext,
                            IConfiguration configuration,
                            IMapper mapper,
                            UserManager<IdentityUser> userManager)
        {
            _dbContext = dbContext;
            _configuration = configuration;
            _mapper = mapper;
            _userManager = userManager;
        }

        public async Task<IdentityResult> Registration(RegistrationModel model)
        {
            var identityUser = _mapper.Map<IdentityUser>(model);
            return await _userManager.CreateAsync(identityUser, model.Password);
        }

        public async Task<IdentityUser> GetUser(string userName, string password = null)
        {
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
                user = await _userManager.FindByEmailAsync(userName);
            // check the credentials
            if (password != null && !await _userManager.CheckPasswordAsync(user, password)) return null;
            return user;
        }

        public async Task<AuthResponseModel> Authenticate(IdentityUser user, string ipAddress)
        {
            var identity = await GetClaimsIdentityByUser(user);
            var jwtToken = GenerateJwtToken(identity);

            var refreshToken = GenerateRefreshToken(user.Id, ipAddress);
            // save refresh token
            _dbContext.RefreshTokens.Add(refreshToken);
            _dbContext.SaveChanges();

            return new AuthResponseModel(user, jwtToken, refreshToken.Token);
        }

        public async Task<AuthResponseModel> RefreshToken(string token, string ipAddress)
        {
            var refreshToken = _dbContext.RefreshTokens.FirstOrDefault(x => x.Token == token);
            // return null if token is no longer active
            if (refreshToken == null || !refreshToken.IsActive) return null;

            var user = await _userManager.FindByIdAsync(refreshToken.UserId);
            if (user == null) return null;

            // replace old refresh token with a new one and save
            var newRefreshToken = GenerateRefreshToken(user.Id, ipAddress);
            refreshToken.Revoked = DateTime.UtcNow;
            refreshToken.RevokedByIp = ipAddress;
            refreshToken.ReplacedByToken = newRefreshToken.Token;
            _dbContext.RefreshTokens.Add(newRefreshToken);
            _dbContext.SaveChanges();

            // generate new jwt
            var identity = await GetClaimsIdentityByUser(user);
            var jwtToken = GenerateJwtToken(identity);

            return new AuthResponseModel(user, jwtToken, newRefreshToken.Token);
        }

        public bool RevokeToken(string token, string ipAddress)
        {
            var refreshToken = _dbContext.RefreshTokens.FirstOrDefault(x => x.Token == token);
            
            // return false if token is not active
            if (refreshToken == null || !refreshToken.IsActive) return false;

            // revoke token and save
            refreshToken.Revoked = DateTime.UtcNow;
            refreshToken.RevokedByIp = ipAddress;
            _dbContext.SaveChanges();

            return true;
        }

        private async Task<ClaimsIdentity> GetClaimsIdentityByUser(IdentityUser user)
        {
            var claims = await _userManager.GetClaimsAsync(user);
            claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
            claims.Add(new Claim(ClaimTypes.Name, user.UserName));
            return await Task.FromResult(new ClaimsIdentity(claims, JwtBearerDefaults.AuthenticationScheme));
        }

        private string GenerateJwtToken(ClaimsIdentity identity)
        {
            var creds = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"])),
                SecurityAlgorithms.HmacSha256);
            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Issuer"],
                notBefore: DateTime.UtcNow,
                claims: identity.Claims,
                expires: DateTime.UtcNow.Add(TimeSpan.FromDays(1)),
                signingCredentials: creds);
            return new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
        }

        private RefreshToken GenerateRefreshToken(string userId, string ipAddress)
        {
            using var rngCryptoServiceProvider = new RNGCryptoServiceProvider();
            var randomBytes = new byte[64];
            rngCryptoServiceProvider.GetBytes(randomBytes);
            return new RefreshToken
            {
                UserId = userId,
                Token = Convert.ToBase64String(randomBytes),
                Expires = DateTime.UtcNow.AddDays(7),
                Created = DateTime.UtcNow,
                CreatedByIp = ipAddress
            };
        }
    }
}