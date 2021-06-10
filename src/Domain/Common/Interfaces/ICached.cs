namespace Domain.Common.Interfaces
{
    public interface ICached
    {
        public string GetCacheTag() => this.GetType().Name;
    }
}
