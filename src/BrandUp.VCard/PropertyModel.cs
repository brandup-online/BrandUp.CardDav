using System.Collections;

namespace BrandUp.CardDav.VCard
{
    /// <summary>
    /// 
    /// </summary>
    public class PropertyModel : IPropertyModel
    {
        IEnumerable<VCardLine> lines;

        string preferred = "";
        IEnumerable<string> values;

        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<string> Values => values;

        /// <summary>
        /// 
        /// </summary>
        public string Preferred => preferred;

        internal PropertyModel(IEnumerable<VCardLine> lines)
        {
            this.lines = lines ?? throw new ArgumentNullException(nameof(lines));
            UpdatePropperties();
        }

        internal PropertyModel(VCardLine firstLine)
        {
            if (firstLine == null)
                throw new ArgumentNullException(nameof(firstLine));

            lines = new List<VCardLine>() { firstLine };
            UpdatePropperties();
        }

        internal void AddLine(VCardLine line)
        {
            lines = lines.Append(line);
            UpdatePropperties();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            var other = obj as PropertyModel;

            return lines.SequenceEqual(other.lines);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        #region IEnumerable members

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerator<VCardLine> GetEnumerator()
        {
            return lines.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region Helpers

        void UpdatePropperties()
        {
            values = lines.Select(l => l.Value).ToArray();
            SetPreferred();
        }

        void SetPreferred()
        {

        }

        #endregion
    }

    /// <summary>
    /// 
    /// </summary>
    public interface IPropertyModel : IEnumerable<VCardLine>
    {
        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<string> Values { get; }

        /// <summary>
        /// 
        /// </summary>
        public string Preferred { get; }
    }
}
