using System.Collections.Generic;
using TelephoneBookAssignment.Entities;

namespace TelephoneBookAssignment.Jwt
{
    public interface ITokenHelper
    {
        AccessToken CreateToken(User user, List<UserClaim> userClaims);
    }
}