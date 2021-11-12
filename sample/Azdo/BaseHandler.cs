using Glow.Azdo.Authentication;

namespace Glow.Sample.Azdo;

public abstract class BaseHandler
{
    protected readonly AzdoClients clients;

    public BaseHandler(AzdoClients clients)
    {
        this.clients = clients;
    }
}