using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;

namespace Services.Contracts
{
    public interface ISizesService
    {
        ICollection<Size> GetSizes();

        Task Create(ProductSize size);
    }
}
