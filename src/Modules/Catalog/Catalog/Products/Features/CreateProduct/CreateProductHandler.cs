using Microsoft.Extensions.Logging;

namespace Catalog.Products.Features.CreateProduct;

public record CreateProductCommand(ProductDto Product) : ICommand<CreateProductResult>;

public record CreateProductResult(Guid Id);

public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(c => c.Product.Name).NotEmpty().WithMessage("Name is required.");
        RuleFor(c => c.Product.Category).NotEmpty().WithMessage("Category is required.");
        RuleFor(c => c.Product.ImageFile).NotEmpty().WithMessage("ImageFile is required.");
        RuleFor(c => c.Product.Price).GreaterThan(0).WithMessage("Price must be larger than 0.");
    }
}

public class CreateProductHandler(
    CatalogDbContext dbContext,
    ILogger<CreateProductHandler> logger)
    : ICommandHandler<CreateProductCommand, CreateProductResult>
{
    public async Task<CreateProductResult> Handle(CreateProductCommand command,
        CancellationToken cancellationToken)
    {
        //1. Create Product entity from commnad object
        //2. Save to database
        //3. Return result
        
        //logging part
        logger.LogInformation("CreateProductCommandHandler. Handle called with {@Command}", command);
        
        //actual logic
        var product = CreateNewProduct(command.Product);

        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new CreateProductResult(product.Id);
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