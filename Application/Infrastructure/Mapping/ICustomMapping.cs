using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;

namespace Application.Infrastructure.Mapping
{
    public interface ICustomMapping
    {
        void ConfigureMapping(Profile profile);
    }
}
