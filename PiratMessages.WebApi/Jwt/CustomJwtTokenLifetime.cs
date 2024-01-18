using Microsoft.IdentityModel.Tokens;

namespace PiratMessages.WebApi.Jwt
{
    public class CustomJwtTokenLifetime
    {
        public static bool CustomLifetimeValidator(DateTime? notBefore, DateTime? expires,
                                                   SecurityToken tokenToValidate, TokenValidationParameters parameters)
        {
            if (expires != null)
            {
                return expires > DateTime.UtcNow;
            }
            return false;
        }
    }
}
