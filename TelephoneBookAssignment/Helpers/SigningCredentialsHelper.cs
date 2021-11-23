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
// This class defines the which security key and security algorithm will be used in managing system of Jwt