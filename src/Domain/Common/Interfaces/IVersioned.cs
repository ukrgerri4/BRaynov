namespace Domain.Common.Interfaces
{
    public interface IVersioned
    {
        byte[] RowVersion { get; set; }
    }
}
