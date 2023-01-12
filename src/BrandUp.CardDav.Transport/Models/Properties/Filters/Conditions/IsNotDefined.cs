using BrandUp.CardDav.VCard;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace BrandUp.CardDav.Transport.Models.Properties.Filters.Conditions
{
    /// <summary>
    /// <see href="https://www.rfc-editor.org/rfc/rfc6352.html#section-10.5.3"/>
    /// </summary>
    public class IsNotDefined : ICondition
    {
        #region IDavProperty member

        /// <summary>
        /// Xml property name
        /// </summary>
        public string Name => "is-not-defined";

        /// <summary>
        /// Xml property namespace
        /// </summary>
        public string Namespace => "urn:ietf:params:xml:ns:carddav";

        #endregion

        #region ICondition member

        /// <summary>
        /// If this function have been invoked, then property is defined.
        /// </summary>
        /// <param name="line"></param>
        /// <returns>Always false</returns>
        public bool Check(VCardLine line) => false;

        #endregion

        #region IXmlSerializable member

        XmlSchema IXmlSerializable.GetSchema() => null;

        void IXmlSerializable.ReadXml(XmlReader reader)
        {

        }

        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement(Name, Namespace);
            writer.WriteEndElement();
        }

        #endregion
    }
}
