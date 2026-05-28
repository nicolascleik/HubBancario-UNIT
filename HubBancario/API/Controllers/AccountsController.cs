using System;
using System.Threading.Tasks;
using HubBancario.Application.Commands.Account.CreateAccount;
using HubBancario.Application.Commands.Account.DeleteAccount;
using HubBancario.Application.Commands.Account.UpdateAccount;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HubBancario.API.Controllers
{
    [ApiController]
    [Route("api/accounts")]
    [Produces("application/json")]
    public class AccountsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AccountsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Cadastra uma nova conta lojista no Hub Bancário.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateAccountCommand command)
        {
            var id = await _mediator.Send(command);
            
            // Retorna o HTTP 201 Created indicando onde o recurso foi gerado
            return CreatedAtAction(nameof(Create), new { id }, new { Id = id });
        }

        /// <summary>
        /// Atualiza os dados cadastrais de uma conta existente.
        /// </summary>
        [HttpPut("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateAccountCommand command)
        {
            // Defesa básica de borda para garantir integridade do ID da rota
            if (id != command.Id)
            {
                return BadRequest(new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "Inconsistência de Dados",
                    Detail = "O ID fornecido na URL do endpoint não corresponde ao ID enviado no corpo do payload JSON."
                });
            }

            await _mediator.Send(command);
            return NoContent(); // Padrão REST para atualizações bem-sucedidas sem retorno de corpo
        }

        /// <summary>
        /// Realiza a desativação ou remoção lógica da conta lojista.
        /// </summary>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _mediator.Send(new DeleteAccountCommand { Id = id });
            return NoContent();
        }
    }
}