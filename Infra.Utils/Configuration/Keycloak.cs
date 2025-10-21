namespace Infra.Utils.Configuration;

public class Keycloak
{
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public string ClientId { get; set; } = string.Empty;
    public string RealmId { get; set; } = string.Empty;
}