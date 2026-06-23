namespace BuildingBlocks.Application.Events;

public record ConfirmEmailRequestedEvent(string Email , string ConfirmationUrl);