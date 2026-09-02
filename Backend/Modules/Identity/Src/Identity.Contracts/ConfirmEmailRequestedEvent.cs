namespace Identity.Contracts;

public record ConfirmEmailRequestedEvent(string FullName, string Email, string ConfirmationUrl);