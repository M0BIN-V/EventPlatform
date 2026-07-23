namespace BuildingBlocks.Application.Events;

public record ConfirmEmailRequestedEvent(string FullName, string Email , string ConfirmationUrl);