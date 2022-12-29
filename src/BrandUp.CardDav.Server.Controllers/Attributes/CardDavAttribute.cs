using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;

namespace BrandUp.CardDav.Server.Attributes
{
    /// <summary>
    /// Defines a action that supports Dav methods.
    /// </summary>
    public abstract class CardDavAttribute : ConsumesAttribute, IRouteTemplateProvider, IActionHttpMethodProvider
    {
        /// <summary>
        /// Attribute constructor.
        /// </summary>
        /// <param name="method">Method name</param>
        /// <param name="template">Route template</param>
        /// <param name="contentType">Supported content type</param>
        /// <param name="otherContentTypes">Supported content types</param>
        /// <exception cref="ArgumentNullException"></exception>
        public CardDavAttribute(string method, string template, string contentType, params string[] otherContentTypes) : this(new string[1] { method }, contentType, otherContentTypes)
        {
            if (template == null)
            {
                throw new ArgumentNullException("template");
            }

            Template = template;
        }
        /// <summary>
        /// Attribute constructor
        /// </summary>
        /// <param name="method">Method name</param>
        /// <param name="contentType">Supported content type</param>
        /// <param name="otherContentTypes">Supported content types</param>
        public CardDavAttribute(string method, string contentType, params string[] otherContentTypes) : this(new string[1] { method }, contentType, otherContentTypes)
        { }

        private CardDavAttribute(string[] method, string contentType, params string[] otherContentTypes) : base(contentType, otherContentTypes)
        {
            HttpMethods = method.Select(m => m.ToUpperInvariant()).ToList();
        }

        #region IRouteTemplateProvider

        private int? order;

        /// <summary>
        /// The route template. May be null.
        /// </summary>
        public string Template { get; }

        /// <summary>
        /// Gets the route order. The order determines the order of route execution. Routes
        ///     with a lower order value are tried first. When a route doesn't specify a value,
        ///     it gets a default value of 0. A null value for the Order property means that
        ///    the user didn't specify an explicit order for the route.
        /// </summary>
        public int? Order
        {
            get
            {
                return order ?? 0;
            }
            set
            {
                order = value;
            }
        }

        int? IRouteTemplateProvider.Order => order;

        /// <summary>
        ///     Gets the route name. The route name can be used to generate a link using a specific
        ///     route, instead of relying on selection of a route based on the given set of route
        ///     values.
        /// </summary>
        public string Name { get; set; }

        #endregion

        #region IActionHttpMethodProvider

        /// <summary>
        /// List of supported methods
        /// </summary>
        public IEnumerable<string> HttpMethods { get; }

        #endregion
    }
}
