using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace VecoBackend.Utils;

public class Hash
{
  
    public static KeyValuePair<string, string> GenerateHash(string s)
    {
        var salt = new byte[128 / 8];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(salt);
        }

        var stringSalt = Convert.ToBase64String(salt);
        var hash = Convert.ToBase64String(KeyDerivation.Pbkdf2(s, salt, KeyDerivationPrf.HMACSHA1, 1000, 256 / 8));
        return new KeyValuePair<string, string>(hash, stringSalt);
    }

    public static string GenerateHashFromSalt(string s, string strSalt) => Convert.ToBase64String(
        KeyDerivation.Pbkdf2(s, Convert.FromBase64String(strSalt), KeyDerivationPrf.HMACSHA1, 1000, 256 / 8));
}