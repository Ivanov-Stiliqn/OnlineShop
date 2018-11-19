using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Application.Areas.Shopping.Controllers
{
    [Authorize]
    [Area("Shopping")]
    public class ShoppingAreaController: Controller
    {
    }
}
