using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Models;

namespace Services.Contracts
{
    public interface ISizesService
    {
        IQueryable<Size> GetSizes();

        void Create(ProductSize size);
    }
}
