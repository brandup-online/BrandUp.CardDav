using BrandUp.CardDav.Transport.Abstract.Handling;
using BrandUp.CardDav.Transport.Abstract.Properties;
using BrandUp.CardDav.Transport.Models;
using BrandUp.CardDav.Transport.Models.Properties;
using BrandUp.CardDav.Transport.Server.Binding;
using Microsoft.Extensions.DependencyInjection;

namespace BrandUp.CardDav.Transport.Handling.Context
{
    /// <summary>
    /// 
    /// </summary>
    public class HandlerContext : IHandlerContext
    {
        private Dictionary<IDavProperty, Func<IPropertyHandler>> handlers;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public HandlerContext(IServiceProvider serviceProvider)
        {
            if (serviceProvider == null)
                throw new ArgumentNullException(nameof(serviceProvider));

            handlers = new(new PropertyComparer())
            {
                { Prop.ResourceType, () => serviceProvider.GetRequiredService<ResourcetypeHandler>() },
                { Prop.ETag, () => serviceProvider.GetRequiredService<EtagHandler>() },
                { Prop.CTag, () => serviceProvider.GetRequiredService<CtagHandler>() },
                { Prop.PrincipalUrl, () => serviceProvider.GetRequiredService<PrincipalUrlHandler>() },
                { Prop.CurrentUserPrincipal, () => serviceProvider.GetRequiredService<CurrentUserPrincipalHandler>() }
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="prop"></param>
        /// <returns></returns>
        public IPropertyHandler GetHandler(IDavProperty prop)
        {
            if (handlers.TryGetValue(prop, out var func))
            {
                return func();
            }
            return null;
        }
    }
}
