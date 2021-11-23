using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using TelephoneBookAssignment.Entities;
using TelephoneBookAssignment.Extensions;
using TelephoneBookAssignment.Jwt;

namespace TelephoneBookAssignment.Helpers
{
    public class JwtHelper : ITokenHelper
    {
        public IConfiguration Configuration { get; }
        private readonly TokenOptions _tokenOptions;
        private DateTime _accessTokenExpiration;

        public JwtHelper(IConfiguration configuration)
        {
            Configuration = configuration;
            _tokenOptions = Configuration.GetSection("TokenOptions").Get<TokenOptions>(); // From DI
        }

        public AccessToken CreateToken(User user, List<UserClaim> userClaims)
        {
            _accessTokenExpiration = DateTime.Now.AddMinutes(_tokenOptions.AccessTokenExpiration); // 15 minutes
            var securityKey = SecurityKeyHelper.CreateSecurityKey(_tokenOptions.SecurityKey); // symmetric key
            var signingCredentials = SigningCredentialsHelper.CreateSigningCredentials(securityKey); // Security key + HmacSha512Signature

            var jwt = CreateJwtSecurityToken(_tokenOptions, user, signingCredentials, userClaims);

            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            var token = jwtSecurityTokenHandler.WriteToken(jwt);

            return new AccessToken
            {
                Token = token,
                Expiration = _accessTokenExpiration
            };
        }

        public JwtSecurityToken CreateJwtSecurityToken(TokenOptions tokenOptions, User user, SigningCredentials signingCredentials, List<UserClaim> userClaims)
        {
            var jwt = new JwtSecurityToken(
                issuer: tokenOptions.Issuer,
                audience: tokenOptions.Audience,
                expires: _accessTokenExpiration,
                notBefore: DateTime.Now,
                claims: SetClaims(user, userClaims),
                signingCredentials: signingCredentials
            );

            return jwt;
        }

        private IEnumerable<Claim> SetClaims(User user, List<UserClaim> userClaims)
        {
            var claims = new List<Claim>();
            // From ClaimsExtension
            claims.AddNameIdentifier(user.ObjectId.ToString());
            claims.AddEmail(user.Email);
            claims.AddName($"{user.Username}");
            claims.AddRoles(userClaims.Select(c => c.Claim).ToArray());

            return claims;
        }
    }
}