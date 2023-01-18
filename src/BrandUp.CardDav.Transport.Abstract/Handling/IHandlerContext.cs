using BrandUp.CardDav.Transport.Abstract.Handling;
using BrandUp.CardDav.Transport.Abstract.Properties;

namespace BrandUp.CardDav.Transport.Server.Binding
{


    public interface IHandlerContext
    {
        public IPropertyHandler GetHandler(IDavProperty prop);
        public IDictionary<IDavProperty, IPropertyHandler> All();
    }
}