using Shared.Exceptions;

namespace Catalog.Products.Exceptions;

public class ProductNotFoundException(Guid id) : NotFoundException("Product", id);