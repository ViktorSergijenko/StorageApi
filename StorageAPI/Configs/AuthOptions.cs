using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StorageAPI.Configs
{
    public class AuthOptions
    {
        public const string ISSUER = "MyAuthServer"; // издатель токена
        public const string AUDIENCE = "http://localhost:4200/"; // потребитель токена
        const string KEY = "jrcbvbhjydsquhfknjn,fnk18021";   // ключ для шифрации
        public const int LIFETIME = 12000; // время жизни токена - 50 минут
        public static SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(KEY));
        }
    }
}
