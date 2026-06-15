using FluentAssertions;
using HubBancario.Domain.Aggregates.PixCharge;
using HubBancario.Domain.Exceptions;
using HubBancario.Domain.ValueObjects;

namespace HubBancario.Tests.Unit.Domain;

public class PixChargeTests
{
    [Fact]
    public void Create_ShouldCreateActivePixCharge_WhenDataIsValid()
    {
        var txId = TxId.From("HUB123456");

        var pixCharge = PixCharge.Create(
            txId,
            Guid.NewGuid(),
            PixChargeType.Cob,
            "000201010212",
            "{}");

        pixCharge.TxId.Should().Be(txId);
        pixCharge.Status.Should().Be(PixChargeStatus.Active);
        pixCharge.ChargeType.Should().Be(PixChargeType.Cob);
        pixCharge.Emv.Should().Be("000201010212");
    }

    [Fact]
    public void Create_ShouldThrowDomainException_WhenTxIdIsNull()
    {
        var act = () => PixCharge.Create(
            null!,
            Guid.NewGuid(),
            PixChargeType.Cob,
            "000201010212",
            "{}");

        act.Should().Throw<DomainException>()
            .WithMessage("O TxId é obrigatório.");
    }

    [Fact]
    public void Create_ShouldThrowDomainException_WhenInvoiceIdIsEmpty()
    {
        var act = () => PixCharge.Create(
            TxId.From("HUB123456"),
            Guid.Empty,
            PixChargeType.Cob,
            "000201010212",
            "{}");

        act.Should().Throw<DomainException>()
            .WithMessage("O InvoiceId é obrigatório.");
    }

    [Fact]
    public void UpdateStatus_ShouldThrowDomainException_WhenExpiredReturnsToActive()
    {
        var pixCharge = PixCharge.Create(
            TxId.From("HUB123456"),
            Guid.NewGuid(),
            PixChargeType.Cob,
            "000201010212",
            "{}");

        pixCharge.UpdateStatus(PixChargeStatus.Expired);

        var act = () => pixCharge.UpdateStatus(PixChargeStatus.Active);

        act.Should().Throw<DomainException>()
            .WithMessage("Uma cobrança expirada não pode voltar a ficar ativa.");
    }
}