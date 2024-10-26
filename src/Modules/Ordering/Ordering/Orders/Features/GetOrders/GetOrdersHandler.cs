using Mapster;
using Microsoft.EntityFrameworkCore;
using Ordering.Data;
using Ordering.Orders.Dtos;
using Shared.Pagination;
using SharedContracts.CQRS;

namespace Ordering.Orders.Features.GetOrders;

public record GetOrdersQuery(PaginationRequest PaginationRequest) : IQuery<GetOrdersResult>;

public record GetOrdersResult(PaginatedResult<OrderDto> Orders);

public class GetOrdersHandler : IQueryHandler<GetOrdersQuery, GetOrdersResult>
{
    private readonly OrderingDbContext _dbContext;

    public GetOrdersHandler(OrderingDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<GetOrdersResult> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
    {
        var pageIndex = request.PaginationRequest.PageIndex;
        var pageSize = request.PaginationRequest.PageSize;

        var totalCount = await _dbContext.Orders.LongCountAsync(cancellationToken);

        var orders = _dbContext.Orders.AsNoTracking()
            .Include(x => x.Items)
            .OrderBy(o => o.OrderName)
            .Skip(pageIndex * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var orderDtos = orders.Adapt<List<OrderDto>>();

        return new GetOrdersResult(
            new PaginatedResult<OrderDto>(pageIndex, pageIndex, totalCount, orderDtos)
        );
    }
}