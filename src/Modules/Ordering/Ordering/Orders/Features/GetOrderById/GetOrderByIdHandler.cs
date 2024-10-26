using Mapster;
using Microsoft.EntityFrameworkCore;
using Ordering.Data;
using Ordering.Orders.Dtos;
using Shared.Exceptions;
using SharedContracts.CQRS;

namespace Ordering.Orders.Features.GetOrderById;

public record GetOrderByIdQuery(Guid OrderId) : IQuery<GetOrderByIdResult>;

public record GetOrderByIdResult(OrderDto Order);

public class GetOrderByIdHandler : IQueryHandler<GetOrderByIdQuery, GetOrderByIdResult>
{
    private readonly OrderingDbContext _dbContext;

    public GetOrderByIdHandler(OrderingDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<GetOrderByIdResult> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var order = await _dbContext.Orders
            .AsNoTracking()
            .Include(o => o.Items)
            .SingleOrDefaultAsync(o => o.Id == request.OrderId, cancellationToken)
            ?? throw new NotFoundException("Order not found.");

        var orderDto = order.Adapt<OrderDto>();

        return new GetOrderByIdResult(orderDto);
    }
}