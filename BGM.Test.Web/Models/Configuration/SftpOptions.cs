namespace BGM.Test.Web.Models.Configuration;

public record SftpOptions()
{
    public required string Username { get; init; }
    public required string Password { get; init; }
    public required string Host { get; init; }
    public required int Port { get; init; }
}