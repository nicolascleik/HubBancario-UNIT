using System;
using HubBancario.Application.Interfaces;
using HubBancario.Infrastructure.BankAdapters.Itau;
using Microsoft.Extensions.DependencyInjection;

namespace HubBancario.Infrastructure.BankAdapters
{
    public class BankAdapterFactory : IBankAdapterFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public BankAdapterFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IBankPixAdapter GetAdapter(string bankId)
        {
            // O código COMPE do Itaú é 341. 
            // Utilizamos o ServiceProvider para resgatar a classe do contêiner de injeção do .NET,
            // garantindo que o HttpClient e o Logger sejam injetados automaticamente.
            return bankId switch
            {
                "341" => _serviceProvider.GetRequiredService<ItauPixAdapter>(),
                
                // Futuramente podemos adicionar "001" para Banco do Brasil, "237" Bradesco, etc.
                _ => throw new NotSupportedException($"O banco com o código COMPE '{bankId}' ainda não é suportado pelo Hub Bancário.")
            };
        }
    }
}