using FluentAssertions;
using HubBancario.Application.Commands.CreateInvoice;
using HubBancario.Domain.Aggregates.Account;
using HubBancario.Domain.Aggregates.Invoice;
using HubBancario.Domain.Exceptions;
using HubBancario.Domain.Repositories;
using HubBancario.Domain.ValueObjects;
using Moq;

namespace HubBancario.Tests.Unit.Application;

public class CreateInvoiceHandlerTests
{
    [Fact]
    public async Task Handle_ShouldCreateInvoice_WhenAccountExists()
    {
        var account = Account.Create(
            Guid.NewGuid(),
            Document.Create("529.982.247-25"),
            "ITAU",
            "12345-6",
            "0001");

        var invoiceRepository = new Mock<IInvoiceRepository>();
        var accountRepository = new Mock<IAccountRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();

        accountRepository
            .Setup(repository => repository.GetByIdAsync(account.Id))
            .ReturnsAsync(account);

        var handler = new CreateInvoiceHandler(
            invoiceRepository.Object,
            accountRepository.Object,
            unitOfWork.Object);

        var command = new CreateInvoiceCommand
        {
            AccountId = account.Id,
            Amount = 100,
            DueDate = DateTime.UtcNow.AddDays(10),
            ExternalReference = "REF-001"
        };

        var result = await handler.Handle(command, CancellationToken.None);

        result.Should().NotBe(Guid.Empty);

        invoiceRepository.Verify(
            repository => repository.AddAsync(It.IsAny<Invoice>()),
            Times.Once);

        unitOfWork.Verify(
            unitOfWork => unitOfWork.CommitAsync(),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowDomainException_WhenAccountDoesNotExist()
    {
        var invoiceRepository = new Mock<IInvoiceRepository>();
        var accountRepository = new Mock<IAccountRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();

        accountRepository
            .Setup(repository => repository.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Account?)null);

        var handler = new CreateInvoiceHandler(
            invoiceRepository.Object,
            accountRepository.Object,
            unitOfWork.Object);

        var command = new CreateInvoiceCommand
        {
            AccountId = Guid.NewGuid(),
            Amount = 100,
            DueDate = DateTime.UtcNow.AddDays(10),
            ExternalReference = "REF-001"
        };

        var act = async () => await handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<DomainException>()
            .WithMessage("Conta não encontrada.");

        invoiceRepository.Verify(
            repository => repository.AddAsync(It.IsAny<Invoice>()),
            Times.Never);

        unitOfWork.Verify(
            unitOfWork => unitOfWork.CommitAsync(),
            Times.Never);
    }
}