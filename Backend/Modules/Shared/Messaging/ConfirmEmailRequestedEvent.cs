namespace Messaging;

public record ConfirmEmailRequestedEvent(string FullName, string Email, string ConfirmationUrl);