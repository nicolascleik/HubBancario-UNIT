using System;
using System.Threading.Tasks;
using HubBancario.Application.Commands.ClientSecret.RevokeClientSecret;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HubBancario.API.Controllers
{
    [ApiController]
    [Route("api/client-secrets")]
    [Produces("application/json")]
    public class ClientSecretsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ClientSecretsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Revoga imediatamente uma credencial (Client Secret) por motivos de segurança ou suspeita de vazamento.
        /// </summary>
        /// <remarks>
        /// Esta operação executa uma inativação lógica do segredo no banco de dados, interrompendo qualquer autenticação futura de imediato.
        /// </remarks>
        /// <param name="id">Identificador único (Guid) da credencial que será revogada.</param>
        [HttpPost("{id:guid}/revoke")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Revoke(Guid id)
        {
            // Vincula o ID que veio na rota HTTP diretamente ao comando do MediatR
            var command = new RevokeClientSecretCommand { Id = id };
            
            await _mediator.Send(command);
            
            // Retorna 204 No Content, pois a credencial foi derrubada com sucesso e não há corpo de resposta
            return NoContent();
        }
    }
}