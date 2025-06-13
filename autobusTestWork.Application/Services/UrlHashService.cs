using System.Security.Cryptography;
using System.Text;

namespace autobusTestWork.Application.Services;

public class UrlHashService
{
    public string HashUrl(string url, string baseUrl)
    {
        using SHA256 sha256 = SHA256.Create();

        byte[] hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(url));

        StringBuilder hashStr = new StringBuilder();
        foreach (byte b in hash.Take(8))
        {
            hashStr.Append(b.ToString("x2"));
        }

        return $"{baseUrl}/link/{hashStr}";
    }
}
