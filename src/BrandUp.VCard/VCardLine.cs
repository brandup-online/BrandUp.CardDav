namespace BrandUp.CardDav.VCard
{
    /// <summary>
    /// 
    /// </summary>
    public class VCardLine
    {
        /// <summary>
        /// 
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<VCardParameter> Parameters { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            var other = obj as VCardLine;

            if (other == null) return false;

            return other.Value.Equals(Value) && Enumerable.SequenceEqual(Parameters.OrderBy(_ => _, new ParamComparer()), other.Parameters.OrderBy(_ => _, new ParamComparer()));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        private class ParamComparer : IComparer<VCardParameter>
        {
            public int Compare(VCardParameter x, VCardParameter y)
            {
                return x.Value.CompareTo(y.Value);
            }
        }
    }
}
