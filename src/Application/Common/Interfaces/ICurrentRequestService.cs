namespace Application.Common.Interfaces
{
    public interface ICurrentRequestService
    {
        int UserId { get; }
        string RequestId { get; }
    }
}
