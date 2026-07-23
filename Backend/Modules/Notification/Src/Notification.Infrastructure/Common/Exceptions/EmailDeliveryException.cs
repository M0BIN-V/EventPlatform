namespace Notification.Infrastructure.Common.Exceptions;

public class EmailDeliveryException(string message) : Exception(message);