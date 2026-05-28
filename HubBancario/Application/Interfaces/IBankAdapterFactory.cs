namespace HubBancario.Application.Interfaces
{
    public interface IBankAdapterFactory
    {
        IBankPixAdapter GetAdapter(string bankId);
    }
}
