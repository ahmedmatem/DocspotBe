using DocSpot.Infrastructure.Data.Types;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DocSpot.WebAPI.Areas.Admin.Controllers.Api
{
    [ApiController]
    [Authorize(Roles = Role.Admin)]
    public abstract class BaseAdminController : ControllerBase
    {
    }
}
