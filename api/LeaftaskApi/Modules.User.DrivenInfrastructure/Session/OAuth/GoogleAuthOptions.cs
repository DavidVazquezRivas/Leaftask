namespace Modules.Users.DrivenInfrastructure.Session.OAuth;

public class GoogleAuthOptions
{
    public const string SectionName = "Session:OAuth:Google";
    public string ClientId { get; init; } = string.Empty;
}
