using DragonAsia.SalesMan.Models;

namespace DragonAsia.SalesMan.Event
{
    class ItemDialogFinishedEvent { }
    class ItemStorageUpdatedEvent { }
    class ItemSelectedEvent { public Item Entity { get; set; } }
}
