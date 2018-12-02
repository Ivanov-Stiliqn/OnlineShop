using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Application.Areas.Profile.Controllers
{
    [Area("Profile")]
    [Authorize]
    public class ProfileAreaController: Controller
    {
    }
}
