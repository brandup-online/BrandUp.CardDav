﻿using BrandUp.CardDav.Server.Attributes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Primitives;

namespace BrandUp.CardDav.Server.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [ApiController]
    [Route("")]
    public class HomeController : ControllerBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public Task<ActionResult> GetAsync()
        {
            return Task.FromResult((ActionResult)BadRequest());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpOptions]
        public Task<ActionResult> OptionsAsync()
        {
            var allowValues = new List<string>() { "OPTIONS", "GET", "POST", "PUT", "DELETE", "MKCOL", "PROPFIND", "REPORT" };

            var allow = new StringValues(allowValues.ToArray());

            Response.Headers.Add("Allow", allow);
            Response.Headers.Add("DAV", "1, addressbook");

            return Task.FromResult((ActionResult)Ok());
        }

        [CardDavPropfind("/.well-known/carddav")]
        public ActionResult WellKnown()
        {
            return RedirectToAction("Propfind", "Collections", new { Name = User.Identity.Name });
        }
        [HttpGet("/.well-known/carddav")]
        public ActionResult GetWellKnown()
        {
            return RedirectToAction("Propfind", "Collections", new { Name = User.Identity.Name });
        }
    }
}
