using AutoMapper.QueryableExtensions;

namespace Organization.Application.Common.Mapping;

public static class MappingExtensions
{
    public static Task<List<TDestination>> ProjectToListAsync<TDestination>(
        this IQueryable queryable,
        IConfigurationProvider configuration
    )
        where TDestination : class
        => queryable.ProjectTo<TDestination>(configuration)
        .AsNoTracking()
        .ToListAsync();
}
