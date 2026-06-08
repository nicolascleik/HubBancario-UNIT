using System;
using System.Threading.Tasks;
using HubBancario.Application.Commands.Account.CreateAccount;
using HubBancario.Application.Commands.Account.ChangeAccountStatus; 
using HubBancario.Application.Commands.Account.UpdateAccount;
using HubBancario.Application.Queries.Account;
using HubBancario.Application.DTOs;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HubBancario.API.Controllers.v1
{
    [ApiController]
    [Route("api/v1/accounts")]
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
            
            return CreatedAtAction(nameof(Get), new { id = id }, new { Id = id });
        }

        /// <summary>
        /// Consulta os dados de uma conta específica.
        /// </summary>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(AccountDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get(Guid id)
        {
            var query = new GetAccountByIdQuery(id);
            var result = await _mediator.Send(query);

            if (result == null)
                return NotFound();

            return Ok(result);
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
            return NoContent();
        }

        /// <summary>
        /// Altera o status da conta (Ativa ou Inativa). Substitui a exclusão física (Delete).
        /// </summary>
        [HttpPatch("{id:guid}/status")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ChangeStatus(Guid id, [FromBody] ChangeAccountStatusCommand command)
        {
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
            return NoContent();
        }
    }
}