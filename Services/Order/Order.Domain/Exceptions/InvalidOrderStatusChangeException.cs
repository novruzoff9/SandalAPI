using System.Net;

namespace Order.Domain.Exceptions;

internal class InvalidOrderStatusChangeException : DomainException
{
    public string CurrentStatus { get; }
    public string AttemptedStatus { get; }

    public InvalidOrderStatusChangeException(string currentStatus, string attemptedStatus)
        : base($"Order status change from '{currentStatus}' to '{attemptedStatus}' is not allowed.",
            (int)HttpStatusCode.Conflict)
    {
        CurrentStatus = currentStatus;
        AttemptedStatus = attemptedStatus;
    }
}
