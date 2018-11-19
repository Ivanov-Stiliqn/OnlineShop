using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AutoMapper;

namespace Application.Infrastructure.Mapping
{
    public class AutoMapperProfile: Profile
    {
        public AutoMapperProfile()
        {
            var allTypes = AppDomain
                .CurrentDomain
                .GetAssemblies()
                .Where(a => a.GetName().Name.Contains("Application"))
                .SelectMany(a => a.GetTypes());

           allTypes
                .Where(t => t.IsClass && !t.IsAbstract && t.GetInterfaces()
                                .Where(i => i.IsGenericType)
                                .Select(i => i.GetGenericTypeDefinition())
                                .FirstOrDefault() == typeof(IMapFrom<>))
                 .Select(t => new
                {
                    Destination = t,
                    Source = t
                        .GetInterfaces()
                        .Where(i => i.IsGenericType)
                        .Select(i => new
                        {
                            Definition = i.GetGenericTypeDefinition(),
                            Arguments = i.GetGenericArguments()
                        })
                        .Where(i => i.Definition == typeof(IMapFrom<>))
                        .SelectMany(i => i.Arguments)
                        .First()
                })
                .ToList()
                .ForEach(m => CreateMap(m.Source, m.Destination));

            allTypes
                .Where(t => t.IsClass && !t.IsAbstract && typeof(ICustomMapping).IsAssignableFrom(t))
                .Select(Activator.CreateInstance)
                .Cast<ICustomMapping>()
                .ToList()
                .ForEach(c => c.ConfigureMapping(this));

        }
    }
}
