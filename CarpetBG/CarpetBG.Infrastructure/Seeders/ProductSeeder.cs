using CarpetBG.Domain.Entities;
using CarpetBG.Infrastructure.Data;

namespace CarpetBG.Infrastructure.Seeders;

public class ProductSeeder(AppDbContext dbContext) : ISeeder
{
    public async Task SeedAsync()
    {
        List<Product> targetProducts =
        [
            new Product{Id = Guid.NewGuid(), Name = "Машинно изпиране", NormalizedName = "машинно изпиране", OrderBy = 0, Price = 7.9m, ExpressServicePrice = 1.5m*7.9m, Description = "Машинно изпиране на артикул"},
            new Product{Id = Guid.NewGuid(), Name = "Ръчно изпиране", NormalizedName = "ръчно изпиране", OrderBy = 1, Price = 12m, ExpressServicePrice = 1.5m*12m, Description = "Ръчно изпиране на артикул"},
        ];

        var newlyProducts = targetProducts.Where(p => !dbContext.Products.Any(t => t.Name == p.Name)).ToList();

        if (newlyProducts.Count != 0)
        {
            dbContext.Products.AddRange(targetProducts);
            await dbContext.SaveChangesAsync();
        }
    }
}
