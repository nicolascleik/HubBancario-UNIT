using System;
using System.Threading;
using System.Threading.Tasks;
using HubBancario.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace HubBancario.Application.Behaviors
{
    public class TransactionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<TransactionBehavior<TRequest, TResponse>> _logger;

        public TransactionBehavior(IUnitOfWork unitOfWork, ILogger<TransactionBehavior<TRequest, TResponse>> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            try
            {
                var response = await next();
                await _unitOfWork.CommitAsync();
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar {CommandName}. Realizando rollback.", typeof(TRequest).Name);
                throw;
            }
        }
    }
}
