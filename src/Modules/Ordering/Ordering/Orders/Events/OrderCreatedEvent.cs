using Ordering.Orders.Models;
using Shared.DDD;

namespace Ordering.Orders.Events;

public record OrderCreatedEvent(Order Order) : IDomainEvent;