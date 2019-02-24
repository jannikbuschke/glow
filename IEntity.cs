namespace JannikB.AspNetCore.Utils.Module
{
    public interface IEntity<T>
    {
        T Id { get; }
    }
}
