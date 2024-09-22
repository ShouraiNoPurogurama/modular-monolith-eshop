namespace Catalog.Products.Features.UpdateProduct;

public record UpdateProductCommand(ProductDto Product)
    : ICommand<UpdateProductResult>;

public record UpdateProductResult(bool IsSuccess);

public class UpdateProductHandler(CatalogDbContext dbContext)
    : ICommandHandler<UpdateProductCommand, UpdateProductResult>
{
    public async Task<UpdateProductResult> Handle(UpdateProductCommand command, CancellationToken cancellationToken)
    {
        var product = await dbContext.Products
            .FindAsync([command.Product.Id], cancellationToken);

        if (product is null)
        {
            throw new Exception($"Product not found: {command.Product.Id}");
        }

        UpdateProductWithNewValues(product, command.Product);

        dbContext.Products.Update(product);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new UpdateProductResult(true);
    }

    private void UpdateProductWithNewValues(Product product, ProductDto commandProduct)
    {
        product.Update(
            product.Name,
            product.Category,
            product.Description,
            product.ImageFile,
            product.Price
        );
    }
}