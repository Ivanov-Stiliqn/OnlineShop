using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;

namespace Application.Infrastructure.Mapping
{
    public static class MappingExtensions
    {
        public static TDestination Map<TSource, TDestination>(this TSource source)
        {
            return Mapper.Map<TSource, TDestination>(source);
        }
    }
}
