using BrandUp.CardDav.Services;
using Microsoft.AspNetCore.Mvc;

namespace BrandUp.CardDav.Server.Controllers
{
    public abstract class CardDavController : ControllerBase
    {
        readonly protected IResponseService responseService;

        protected CardDavController(IResponseService responseService)
        {
            this.responseService = responseService ?? throw new ArgumentNullException(nameof(responseService));
        }
    }
}
