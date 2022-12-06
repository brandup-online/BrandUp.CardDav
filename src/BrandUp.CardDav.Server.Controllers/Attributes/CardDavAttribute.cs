using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;

namespace BrandUp.CardDav.Server.Attributes
{
    public abstract class CardDavAttribute : ConsumesAttribute, IRouteTemplateProvider, IActionHttpMethodProvider
    {
        public CardDavAttribute(string method, string template, string contentType, params string[] otherContentTypes) : this(new string[1] { method }, contentType, otherContentTypes)
        {
            if (template == null)
            {
                throw new ArgumentNullException("template");
            }

            Template = template;
        }

        public CardDavAttribute(string[] method, string contentType, params string[] otherContentTypes) : base(contentType, otherContentTypes)
        {
            HttpMethods = method.Select(m => m.ToUpperInvariant()).ToList();
        }

        #region IRouteTemplateProvider

        private int? order;

        public string Template { get; }

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

        public string Name { get; set; }

        #endregion

        #region IActionHttpMethodProvider

        public IEnumerable<string> HttpMethods { get; }

        #endregion
    }
}
