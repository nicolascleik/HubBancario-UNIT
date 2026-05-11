namespace BankingHub.Application.Interfaces
{
    public interface IBankAdapterFactory
    {
        IBankPixAdapter GetAdapter(string bankId);
    }
}
