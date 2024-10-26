using FluentValidation;
using Ordering.Data;
using Shared.Exceptions;
using SharedContracts.CQRS;

namespace Ordering.Orders.Features.DeleteOrder;

public record DeleteOrderCommand(Guid OrderId) : ICommand<DeleteOrderResult>;

public record DeleteOrderResult(bool IsSuccess);

public class DeleteOrderCommandValidator : AbstractValidator<DeleteOrderCommand>
{
    public DeleteOrderCommandValidator()
    {
        RuleFor(x => x.OrderId).NotEmpty().WithMessage("Order ID is required.");
    }
}

public class DeleteOrderHandler : ICommandHandler<DeleteOrderCommand, DeleteOrderResult>
{
    private readonly OrderingDbContext _dbContext;

    public DeleteOrderHandler(OrderingDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<DeleteOrderResult> Handle(DeleteOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await _dbContext.Orders.FindAsync(request.OrderId, cancellationToken)
                    ?? throw new NotFoundException("Order not found.");

        _dbContext.Orders.Remove(order);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return new DeleteOrderResult(true);
    }
}