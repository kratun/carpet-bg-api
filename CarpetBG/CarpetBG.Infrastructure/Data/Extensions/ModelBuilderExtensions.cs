using System.Reflection;

using Microsoft.EntityFrameworkCore;

namespace CarpetBG.Infrastructure.Data.Extensions;

public static class ModelBuilderExtensions
{
    public static void ApplyConfigurations(this ModelBuilder modelBuilder)
    {
        // Automatically scan the assembly containing your configurations
        var assemblies = new[] { Assembly.GetExecutingAssembly() };

        // Apply all IEntityTypeConfiguration<T> implementations
        foreach (var assembly in assemblies)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(assembly);
        }
    }
}
