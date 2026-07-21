namespace Identity.Application.Common.Options;

public class RefreshTokenOptions
{
    public int ExpirationDays { get; set; } = 7;
}