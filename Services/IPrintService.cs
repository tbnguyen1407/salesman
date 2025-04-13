using SalesMan.Models;

namespace SalesMan.Services;

public interface IPrintService
{
    bool Print(Order order);
}
