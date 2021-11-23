using Microsoft.IdentityModel.Tokens;

namespace TelephoneBookAssignment.Helpers
{
    public class SigningCredentialsHelper 
    {
        public static SigningCredentials CreateSigningCredentials(SecurityKey securityKey)
        {
            return new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha512Signature);
        }
    }
}