using SalesMan.Models;

namespace SalesMan.Services;

public interface IDialogService
{
    bool ShowConfirmation(string title, string message);
    void ShowMessage(string title, string message);
    void ShowEntityDialog<T>(EntityAction action, T value);
}
