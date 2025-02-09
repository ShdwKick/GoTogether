using HotChocolate.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using GoTogether.Data;
using GoTogether.Repositories.UserRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using KeyNotFoundException = System.Collections.Generic.KeyNotFoundException;

namespace Server.Data.Helpers
{
    public static class Helpers
    {
        private static IServiceProvider _serviceProvider;

        public static void InitializeServiceProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public static string GetTokenFromHeader()
        {
            using var scope = _serviceProvider.CreateScope();
            var httpContextAccessor = scope.ServiceProvider.GetRequiredService<IHttpContextAccessor>();
            
            if (httpContextAccessor == null)
                throw new ArgumentException("ERROR_OCCURRED");

            var httpContext = httpContextAccessor.HttpContext;
            string authorizationHeader = httpContext.Request.Headers["Authorization"];

            if (string.IsNullOrEmpty(authorizationHeader) || !authorizationHeader.StartsWith("Bearer "))
            {
                throw new ArgumentException("INVALID_AUTHORIZATION_HEADER_PROBLEM");
            }

            return authorizationHeader.Substring("Bearer ".Length).Trim();
        }

        public static string ComputeHash(string input)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(input + ConfigurationHelper.GetSalt());
                byte[] hashBytes = sha256.ComputeHash(inputBytes);

                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    builder.Append(hashBytes[i].ToString("x2"));
                }

                return builder.ToString();
            }
        }


        public static JwtSecurityToken GenerateNewToken(string userId, string userRole)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(ConfigurationHelper.GetServerKey()));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, userRole)
            };
            var newToken = new JwtSecurityToken(
                issuer: ConfigurationHelper.GetIssuer(),
                audience: ConfigurationHelper.GetAudience(),
                claims: claims,
                expires: DateTime.Now.AddMinutes(480),
                signingCredentials: credentials
            );
            return newToken;
        }

        public static async Task<UserData> GetUserFromHeader()
        {
            using var scope = _serviceProvider.CreateScope();
            var userRepo = scope.ServiceProvider.GetRequiredService<IUserRepository>();
            
            var token = GetTokenFromHeader();
            if (string.IsNullOrWhiteSpace(token))
                throw new ArgumentException("AUTH_TOKEN_MISSING_PROBLEM", nameof(token));

            var jwtToken = new JwtSecurityTokenHandler().ReadToken(token) as JwtSecurityToken;
            if (jwtToken == null)
                throw new ArgumentException("INVALID_TOKEN_PROBLEM");

            var claim = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub);
            if (claim == null)
                throw new ArgumentException("INVALID_TOKEN_CLAIMS_PROBLEM");

            if (!Guid.TryParse(claim.Value, out var userId))
                throw new ArgumentException("AUTH_TOKEN_CLAIM_INVALID");

            var user = await userRepo.GetUserDataAsync(userId);
            if (user == null)
                throw new KeyNotFoundException("USER_NOT_FOUND_PROBLEM");

            return user;
        }

        public static int GenerateCode() => new Random().Next(100000, 999999);

        public static StringContent GenerateEmailCodeJson(string email, int code = -1)
        {
            if (code < 0)
                code = GenerateCode();

            return new StringContent(
                JsonSerializer.Serialize(new
                {
                    address = email,
                    code = code.ToString()
                }),
                Encoding.UTF8, "application/json"
            );
        }

        public static StringContent GenerateInviteMessageBodyJson(string email, string code)
        {
            return new StringContent(
                JsonSerializer.Serialize(new
                {
                    address = email,
                    code
                }),
                Encoding.UTF8, "application/json");
        }

        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            Regex regex = new Regex(pattern);

            return regex.IsMatch(email);
        }
    }
}