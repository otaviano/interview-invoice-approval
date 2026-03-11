namespace InvoiceApproval.Domain.Handlers;

public abstract class ApprovalHandler
{
    private ApprovalHandler? _next;

    public ApprovalHandler SetNext(ApprovalHandler next)
    {
        _next = next;
        return next;
    }

    public virtual void Handle(ApprovalContext context)
    {
        _next?.Handle(context);
    }
}
