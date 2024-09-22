namespace Catalog.Products.Features.CreateProduct;
public record CreateProductCommand(ProductDto Product) : ICommand<Result>;
public record Result(Guid Id);

public class CreateProductHandler(CatalogDbContext dbContext) : ICommandHandler<CreateProductCommand, Result>
{
    public async Task<Result> Handle(CreateProductCommand createProductCommand, CancellationToken cancellationToken)
    {
        var product = CreateNewProduct(createProductCommand.Product);

        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new Result(product.Id);
    }

    private Product CreateNewProduct(ProductDto productDto)
    {
        var product = Product.Create(
            Guid.NewGuid(),
            productDto.Name,
            productDto.Category,
            productDto.Description,
            productDto.ImageFile,
            productDto.Price
        );

        return product;
    }
}