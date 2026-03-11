using FluentValidation;
using InvoiceApproval.Application.UseCases.DetermineApprovers;
using InvoiceApproval.Domain.Services;
using NSubstitute.ExceptionExtensions;

namespace InvoiceApproval.Tests.Unit.Application.UseCases;

public class DetermineApproversCommandHandlerTests
{
    private readonly IValidator<DetermineApproversCommand> _validator = Substitute.For<IValidator<DetermineApproversCommand>>();
    private readonly IApprovalWorkflowService _workflowService = Substitute.For<IApprovalWorkflowService>();
    private readonly DetermineApproversCommandHandler _sut;

    public DetermineApproversCommandHandlerTests()
    {
        _sut = new DetermineApproversCommandHandler(_validator, _workflowService);
    }

    [Fact]
    public async Task Handle_WithValidCommand_ShouldReturnApprovers()
    {
        var command = new DetermineApproversCommand(5000m, false);
        var expectedApprovers = new List<string> { "Manager", "Director" }.AsReadOnly();

        _workflowService
            .DetermineApprovers(command.Amount, command.IsPreferredVendor)
            .Returns(expectedApprovers);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.Approvers.Should().BeEquivalentTo(expectedApprovers);
    }

    [Fact]
    public async Task Handle_ShouldCallValidatorBeforeService()
    {
        var command = new DetermineApproversCommand(1000m, false);
        var callOrder = new List<string>();

        _validator
            .When(v => v.ValidateAsync(Arg.Any<ValidationContext<DetermineApproversCommand>>(), Arg.Any<CancellationToken>()))
            .Do(_ => callOrder.Add("validator"));

        _workflowService
            .When(s => s.DetermineApprovers(Arg.Any<decimal>(), Arg.Any<bool>()))
            .Do(_ => callOrder.Add("service"));

        _workflowService
            .DetermineApprovers(Arg.Any<decimal>(), Arg.Any<bool>())
            .Returns(new List<string>().AsReadOnly());

        await _sut.Handle(command, CancellationToken.None);

        callOrder.Should().ContainInOrder("validator", "service");
    }

    [Fact]
    public async Task Handle_ShouldPassCorrectParametersToService()
    {
        var command = new DetermineApproversCommand(15000m, true);

        _workflowService
            .DetermineApprovers(Arg.Any<decimal>(), Arg.Any<bool>())
            .Returns(new List<string> { "Director", "VP" }.AsReadOnly());

        await _sut.Handle(command, CancellationToken.None);

        _workflowService.Received(1).DetermineApprovers(15000m, true);
    }

    [Fact]
    public async Task Handle_WhenValidationFails_ShouldThrowValidationException()
    {
        var command = new DetermineApproversCommand(-1m, false);

        _validator
            .ValidateAsync(Arg.Any<ValidationContext<DetermineApproversCommand>>(), Arg.Any<CancellationToken>())
            .Throws(new ValidationException("Validation failed"));

        var act = async () => await _sut.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<ValidationException>();
        _workflowService.DidNotReceive().DetermineApprovers(Arg.Any<decimal>(), Arg.Any<bool>());
    }

    [Fact]
    public async Task Handle_ShouldPassCancellationToken()
    {
        using var cts = new CancellationTokenSource();
        var token = cts.Token;
        var command = new DetermineApproversCommand(500m, false);

        _workflowService
            .DetermineApprovers(Arg.Any<decimal>(), Arg.Any<bool>())
            .Returns(new List<string> { "Manager" }.AsReadOnly());

        await _sut.Handle(command, token);

        await _validator.Received(1).ValidateAsync(
            Arg.Any<ValidationContext<DetermineApproversCommand>>(),
            token);
    }
}
