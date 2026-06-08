using System;
using System.Threading.Tasks;
using HubBancario.Application.Commands.ClientSecret.RevokeClientSecret;
using HubBancario.Application.Queries.ClientCredential;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HubBancario.API.Controllers
{
    [ApiController]
    [Route("api/client-credentials")]
    [Produces("application/json")]
    public class ClientCredentialsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ClientCredentialsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Retorna os dados de uma credencial de cliente pelo seu ID.
        /// </summary>
        /// <remarks>
        /// Consulta a credencial sem expor campos sensíveis como SecretValue ou CertificatePassword.
        /// Útil para verificar se a credencial está ativa (IsValid) antes de operações bancárias.
        /// </remarks>
        /// <param name="id">Identificador único (Guid) da credencial.</param>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _mediator.Send(new GetClientCredentialByIdQuery(id));

            if (result == null)
                return NotFound(new ProblemDetails
                {
                    Status = StatusCodes.Status404NotFound,
                    Title = "Credencial não encontrada",
                    Detail = $"Nenhuma credencial foi encontrada para o ID '{id}'."
                });

            return Ok(result);
        }

        /// <summary>
        /// Revoga imediatamente uma credencial por motivos de segurança ou suspeita de vazamento.
        /// </summary>
        /// <remarks>
        /// Esta operação executa uma inativação lógica da credencial no banco de dados,
        /// interrompendo qualquer autenticação futura de imediato.
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

            // Retorna 204 No Content, pois a credencial foi revogada com sucesso
            return NoContent();
        }
    }
}
