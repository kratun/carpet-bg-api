using CarpetBG.Application.DTOs.Products;
using CarpetBG.Application.Interfaces.Common;
using CarpetBG.Application.Interfaces.Factories;
using CarpetBG.Application.Interfaces.Repositories;
using CarpetBG.Application.Interfaces.Services;
using CarpetBG.Shared;

namespace CarpetBG.Application.Services;

public class ProductService(
    IProductRepository repository,
    IProductFactory productFactory,
    IValidator<ProductDto> productValidator)
    : IProductService
{
    public async Task<Result<List<ProductDto>>> GetAllAsync()
    {
        var entities = await repository.GetAllAsync();

        //await SeedAsync();

        var result = entities.Select(productFactory.CreateFromEntity)
            .ToList();

        return Result<List<ProductDto>>.Success(result);
    }

    public async Task<Result<ProductDto>> GetByIdAsync(Guid id)
    {
        var entity = await repository.GetByIdAsync(id);

        if (entity == null)
        {
            return Result<ProductDto>.Failure("No data found");
        }

        return Result<ProductDto>.Success(productFactory.CreateFromEntity(entity));
    }

    public async Task<Result<Guid>> AddAsync(ProductDto dto)
    {
        var error = productValidator.Validate(dto);
        if (error != null)
        {
            return Result<Guid>.Failure(error);
        }

        var existedEntity = await repository.GetByNameAsync(dto.Name);
        if (existedEntity != null)
        {
            return Result<Guid>.Failure("The name should be unique");
        }

        var entity = productFactory.CreateFromDto(dto);

        await repository.AddAsync(entity);

        return Result<Guid>.Success(entity.Id);
    }

    public async Task<Result<Guid>> UpdateAsync(Guid id, ProductDto dto)
    {
        var error = productValidator.Validate(dto);
        if (error != null)
        {
            return Result<Guid>.Failure(error);
        }

        var entity = await repository.GetByIdAsync(id);
        if (entity == null)
        {
            return Result<Guid>.Failure("Product was not found");
        }

        entity = productFactory.UpdateFromDto(entity, dto);

        await repository.UpdateAsync(entity);

        return Result<Guid>.Success(entity.Id);
    }

    //private async Task<Result<bool>> SeedAsync()
    //{
    //    var data = new List<ProductDto> {

    //        new()
    //        {
    //            Name = "Машинно пране",
    //            Description = "Машинно пране",
    //            Price = 7.90m,
    //            ExpressServicePrice = 7.90m+(7.90m/2),
    //            OrderBy =0

    //        },
    //        new()
    //        {
    //            Name = "Ръчно пране",
    //            Description = "Ръчно пране",
    //            Price = 12.00m,
    //            ExpressServicePrice = 18.00m,
    //            OrderBy =1
    //        }
    //    };

    //    var entities = data.Select(dto => productFactory.CreateFromDto(dto)).ToList();

    //    await repository.AddAsync(entities[0]);
    //    await repository.AddAsync(entities[1]);

    //    return Result<bool>.Success(true);
    //}
}
