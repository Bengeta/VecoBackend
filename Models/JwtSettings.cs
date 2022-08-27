namespace VecoBackend.Models;

public class JwtSettings
{
    public bool ValidateIssuerSigningKey { get; set; }
    public string SecretKey { get; set; }
    public bool ValidateIssuer { get; set; } = true;
    public string Issuer { get; set; }
    public bool ValidateAudience { get; set; } = true;
    public string Audience { get; set; }
    public bool RequireExpirationTime { get; set; }
    public bool ValidateLifetime { get; set; } = true;
}