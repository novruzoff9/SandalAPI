using AutoMapper;
using AutoMapper.QueryableExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Order.Application.Common.Mapping;

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
