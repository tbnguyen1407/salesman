using SalesMan.Models;

namespace SalesMan.Events;

public class EntityDialogOpenEvent<T>(EntityAction action, T? entity)
{
    public EntityAction Action { get; set; } = action;
    public T? Entity { get; set; } = entity;
}
public class EntityDialogFinishedEvent<T> { }
public class EntityStorageUpdatedEvent<T> { }
