namespace MaxDDDDemo.Domain;

public interface IEntity
{
}

public interface IEntity<T>
{
    T Id { get; }
}