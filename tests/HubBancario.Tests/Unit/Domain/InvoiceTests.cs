using FluentAssertions;
using HubBancario.Domain.Aggregates.Invoice;
using HubBancario.Domain.Exceptions;
using HubBancario.Domain.ValueObjects;

namespace HubBancario.Tests.Unit.Domain;

public class InvoiceTests
{
    [Fact]
    public void Create_ShouldCreateOpenInvoice_WhenDataIsValid()
    {
        var invoice = Invoice.Create(
            Guid.NewGuid(),
            Money.BRL(100),
            DateTime.UtcNow.AddDays(5),
            "REF-001");

        invoice.Id.Should().NotBe(Guid.Empty);
        invoice.Status.Should().Be(InvoiceStatus.Open);
        invoice.Amount.Value.Should().Be(100);
        invoice.ExternalReference.Should().Be("REF-001");
    }

    [Fact]
    public void Create_ShouldThrowDomainException_WhenAccountIdIsEmpty()
    {
        var act = () => Invoice.Create(
            Guid.Empty,
            Money.BRL(100),
            DateTime.UtcNow.AddDays(5),
            "REF-001");

        act.Should().Throw<DomainException>()
            .WithMessage("O AccountId é obrigatório.");
    }

    [Fact]
    public void Create_ShouldThrowDomainException_WhenAmountIsZero()
    {
        var act = () => Invoice.Create(
            Guid.NewGuid(),
            Money.BRL(0),
            DateTime.UtcNow.AddDays(5),
            "REF-001");

        act.Should().Throw<DomainException>()
            .WithMessage("O valor da fatura deve ser maior que zero.");
    }

    [Fact]
    public void MarkAsPaid_ShouldSetStatusToPaid_WhenInvoiceIsOpen()
    {
        var invoice = Invoice.Create(
            Guid.NewGuid(),
            Money.BRL(100),
            DateTime.UtcNow.AddDays(5),
            "REF-001");

        invoice.MarkAsPaid();

        invoice.Status.Should().Be(InvoiceStatus.Paid);
    }

    [Fact]
    public void Cancel_ShouldThrowDomainException_WhenInvoiceIsPaid()
    {
        var invoice = Invoice.Create(
            Guid.NewGuid(),
            Money.BRL(100),
            DateTime.UtcNow.AddDays(5),
            "REF-001");

        invoice.MarkAsPaid();

        var act = () => invoice.Cancel();

        act.Should().Throw<DomainException>()
            .WithMessage("Não é possível cancelar uma fatura que já foi paga.");
    }
}