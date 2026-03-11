using InvoiceApproval.Domain.Handlers;
using InvoiceApproval.Domain.Services;

namespace InvoiceApproval.Tests.Unit.Domain;

public class ApprovalWorkflowServiceTests
{
    private readonly ApprovalWorkflowService _sut;

    public ApprovalWorkflowServiceTests()
    {
        var manager = new ManagerApprovalHandler();
        var director = new DirectorApprovalHandler();
        var vp = new VpApprovalHandler();

        manager.SetNext(director).SetNext(vp);
        _sut = new ApprovalWorkflowService(manager);
    }

    [Theory]
    [InlineData(500, false, new[] { "Manager" })]
    [InlineData(999.99, false, new[] { "Manager" })]
    [InlineData(1, false, new[] { "Manager" })]
    public void DetermineApprovers_UnderOneThousand_ShouldReturnManager(
        decimal amount, bool isPreferredVendor, string[] expected)
    {
        var result = _sut.DetermineApprovers(amount, isPreferredVendor);

        result.Should().BeEquivalentTo(expected, options => options.WithStrictOrdering());
    }

    [Theory]
    [InlineData(1000, false, new[] { "Manager", "Director" })]
    [InlineData(5000, false, new[] { "Manager", "Director" })]
    [InlineData(9999.99, false, new[] { "Manager", "Director" })]
    public void DetermineApprovers_BetweenOneAndTenThousand_ShouldReturnManagerAndDirector(
        decimal amount, bool isPreferredVendor, string[] expected)
    {
        var result = _sut.DetermineApprovers(amount, isPreferredVendor);

        result.Should().BeEquivalentTo(expected, options => options.WithStrictOrdering());
    }

    [Theory]
    [InlineData(10000, false, new[] { "Manager", "Director", "VP" })]
    [InlineData(15000, false, new[] { "Manager", "Director", "VP" })]
    [InlineData(100000, false, new[] { "Manager", "Director", "VP" })]
    public void DetermineApprovers_TenThousandOrMore_ShouldReturnAllApprovers(
        decimal amount, bool isPreferredVendor, string[] expected)
    {
        var result = _sut.DetermineApprovers(amount, isPreferredVendor);

        result.Should().BeEquivalentTo(expected, options => options.WithStrictOrdering());
    }

    [Fact]
    public void DetermineApprovers_PreferredVendor_UnderOneThousand_ShouldReturnEmpty()
    {
        var result = _sut.DetermineApprovers(500, isPreferredVendor: true);

        result.Should().BeEmpty();
    }

    [Fact]
    public void DetermineApprovers_PreferredVendor_BetweenOneAndTenThousand_ShouldReturnDirectorOnly()
    {
        var result = _sut.DetermineApprovers(5000, isPreferredVendor: true);

        result.Should().BeEquivalentTo(new[] { "Director" }, options => options.WithStrictOrdering());
    }

    [Fact]
    public void DetermineApprovers_PreferredVendor_TenThousandOrMore_ShouldReturnDirectorAndVP()
    {
        var result = _sut.DetermineApprovers(15000, isPreferredVendor: true);

        result.Should().BeEquivalentTo(new[] { "Director", "VP" }, options => options.WithStrictOrdering());
    }

    [Theory]
    [InlineData(1000)]
    [InlineData(9999.99)]
    public void DetermineApprovers_PreferredVendor_SkipsManager_KeepsHigherLevels(decimal amount)
    {
        var result = _sut.DetermineApprovers(amount, isPreferredVendor: true);

        result.Should().NotContain("Manager");
        result.Should().Contain("Director");
    }

    [Theory]
    [InlineData(10000)]
    [InlineData(50000)]
    public void DetermineApprovers_PreferredVendor_HighAmount_SkipsManagerOnly(decimal amount)
    {
        var result = _sut.DetermineApprovers(amount, isPreferredVendor: true);

        result.Should().NotContain("Manager");
        result.Should().Contain("Director");
        result.Should().Contain("VP");
    }
}
