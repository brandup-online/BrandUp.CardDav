using BrandUp.CardDav.Transport.Abstract.Handling;
using BrandUp.CardDav.Transport.Abstract.Properties;
using BrandUp.CardDav.Transport.Helpers;
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
        private static Dictionary<IDavProperty, Type> handlers = new(new PropertyComparer())
            {
                { Prop.ResourceType, typeof(ResourcetypeHandler)   },
                { Prop.ETag,  typeof(EtagHandler) },
                { Prop.CTag,  typeof(CtagHandler) },
                { Prop.PrincipalUrl, typeof(PrincipalUrlHandler) },
                { Prop.CurrentUserPrincipal, typeof(CurrentUserPrincipalHandler) },
                { new AddressData(), typeof(AddressDataHandler) }
            };

        readonly IServiceProvider serviceProvider;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public HandlerContext(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IDictionary<IDavProperty, IPropertyHandler> All()
        {
            return handlers.ToDictionary(k => k.Key, v =>
            {
                var handler = (IPropertyHandler)serviceProvider.GetRequiredService(v.Value);
                handler.Property = v.Key;
                return handler;
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="prop"></param>
        /// <returns></returns>
        public IPropertyHandler GetHandler(IDavProperty prop)
        {
            if (handlers.TryGetValue(prop, out var type))
            {
                var handler = (IPropertyHandler)serviceProvider.GetRequiredService(type);
                handler.Property = prop;

                return handler;
            }
            return null;
        }
    }
}
