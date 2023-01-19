using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BrandUp.CardDav.Server.Attributes
{
    /// <summary>
    /// Defines an action with Propfind method.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class CardDavPropfindAttribute : CardDavAttribute, IResourceFilter
    {
        /// <summary>
        /// Contructor
        /// </summary>
        public CardDavPropfindAttribute() : base("PROPFIND", contentType: "text/xml", "application/xml")
        { }

        /// <summary>
        /// Contructor
        /// </summary>
        /// <param name="template">Route template</param>
        public CardDavPropfindAttribute(string template) : base("PROPFIND", template, "text/xml", "application/xml")
        {
        }

        public new void OnResourceExecuting(ResourceExecutingContext context)
        {
            if (context.RouteData.Values.TryGetValue("Name", out var userName))
            {
                if (!userName.Equals(context.HttpContext.User.Identity.Name))
                {
                    context.Result = new NotFoundObjectResult(context.ModelState);
                }
            }
        }
    }
}
