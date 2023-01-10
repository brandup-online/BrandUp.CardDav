namespace BrandUp.CardDav.VCard
{
    /// <summary>
    /// 
    /// </summary>
    public class VCardName
    {
        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<string> FamilyNames { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<string> GivenNames { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<string> AdditionalNames { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<string> HonorificPrefixes { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<string> HonorificSuffixes { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="line"></param>
        public VCardName(string line)
        {
            var name = line.Split(';');

            if (name[0] != "")
                FamilyNames = name[0].Split(',');
            else FamilyNames = new List<string>();

            if (name[1] != "")
                GivenNames = name[1].Split(',');
            else GivenNames = new List<string>();

            if (name[2] != "")
                AdditionalNames = name[2].Split(',');
            else AdditionalNames = new List<string>();

            if (name[3] != "")
                HonorificPrefixes = name[3].Split(',');
            else HonorificPrefixes = new List<string>();

            if (name[4] != "")
                HonorificSuffixes = name[4].Split(',');
            else HonorificSuffixes = new List<string>();
        }
    }
}
