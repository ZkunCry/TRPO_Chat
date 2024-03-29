﻿using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Text;

namespace WebApplication1.jwthandler
{
    public class JwtToken
    {
        private static readonly string _secretKey= "300046c07bc4876a8596e83cbb34744d6462d8f7238660ad2815c1a433f90e934934c";

   

        public static string GenerateToken(string username,string id)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_secretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, username),new Claim("id",id) }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
       
    }
    public static class JwtClaimExtensions
    {
        public static string GetValueOrDefault(this System.Collections.Generic.IEnumerable<Claim> claims, string type)
        {
            var claim = claims.FirstOrDefault(c => c.Type == type);
            return claim?.Value;
        }
    }
}
