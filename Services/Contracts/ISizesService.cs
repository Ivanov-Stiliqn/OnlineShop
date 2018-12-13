using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;
using Models.Enums;

namespace Services.Contracts
{
    public interface ISizesService
    {
        ICollection<Size> GetSizes(CategoryType type, Sex sex);

        Task Create(ProductSize size);
    }
}
