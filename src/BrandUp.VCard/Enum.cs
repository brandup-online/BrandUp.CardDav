namespace BrandUp.CardDav.VCard
{
    /// <summary>
    /// 
    /// </summary>
    public enum CardParameter
    {
        /// <summary>
        /// 
        /// </summary>
        LANGUAGE,
        /// <summary>
        /// 
        /// </summary>
        VALUE,
        /// <summary>
        /// 
        /// </summary>
        PREF,
        /// <summary>
        /// 
        /// </summary>
        ALTID,
        /// <summary>
        /// 
        /// </summary>
        PID,
        /// <summary>
        /// 
        /// </summary>
        TYPE,
        /// <summary>
        /// 
        /// </summary>
        MEDIATYPE,
        /// <summary>
        /// 
        /// </summary>
        CALSCALE,
        /// <summary>
        /// 
        /// </summary>
        SORTAS,
        /// <summary>
        /// 
        /// </summary>
        GEO,
        /// <summary>
        /// 
        /// </summary>
        TZ
    }

    /// <summary>
    /// 
    /// </summary>
    public enum CardProperty
    {    /// <summary>
         /// 
         /// </summary>
        SOURCE,
        /// <summary>
        /// 
        /// </summary>
        KIND,
        /// <summary>
        /// 
        /// </summary>
        XML,
        /// <summary>
        /// 
        /// </summary>
        FN,
        /// <summary>
        /// 
        /// </summary>
        N,
        /// <summary>
        /// 
        /// </summary>
        NICKNAME,
        /// <summary>
        /// 
        /// </summary>
        PHOTO,
        /// <summary>
        /// 
        /// </summary>
        BDAY,
        /// <summary>
        /// 
        /// </summary>
        ANNIVERSARY,
        /// <summary>
        /// 
        /// </summary>
        GENDER,
        /// <summary>
        /// 
        /// </summary>
        ADR,
        /// <summary>
        /// 
        /// </summary>
        TEL,
        /// <summary>
        /// 
        /// </summary>
        EMAIL,
        /// <summary>
        /// 
        /// </summary>
        IMPP,
        /// <summary>
        /// 
        /// </summary>
        LANG,
        /// <summary>
        /// 
        /// </summary>
        TZ,
        /// <summary>
        /// 
        /// </summary>
        GEO,
        /// <summary>
        /// 
        /// </summary>
        TITLE,
        /// <summary>
        /// 
        /// </summary>
        ROLE,
        /// <summary>
        /// 
        /// </summary>
        LOGO,
        /// <summary>
        /// 
        /// </summary>
        ORG,
        /// <summary>
        /// 
        /// </summary>
        MEMBER,
        /// <summary>
        /// 
        /// </summary>
        RELATED,
        /// <summary>
        /// 
        /// </summary>
        CATEGORIES,
        /// <summary>
        /// 
        /// </summary>
        NOTE,
        /// <summary>
        /// 
        /// </summary>
        PRODID,
        /// <summary>
        /// 
        /// </summary>
        REV,
        /// <summary>
        /// 
        /// </summary>
        SOUND,
        /// <summary>
        /// 
        /// </summary>
        UID,
        /// <summary>
        /// 
        /// </summary>
        CLIENTPIDMAP,
        /// <summary>
        /// 
        /// </summary>
        URL,
        /// <summary>
        /// 
        /// </summary>
        VERSION,
        /// <summary>
        /// 
        /// </summary>
        KEY,
        /// <summary>
        /// 
        /// </summary>
        FBURL,
        /// <summary>
        /// 
        /// </summary>
        CALADRURI,
        /// <summary>
        /// 
        /// </summary>
        CALURI
    }

    /// <summary>
    /// 
    /// </summary>
    public enum Kind
    {
        /// <summary>
        /// 
        /// </summary>
        Work,
        /// <summary>
        /// 
        /// </summary>
        Home
    }
    /// <summary>
    /// 
    /// </summary>
    public enum TelType
    {
        /// <summary>
        /// 
        /// </summary>
        Text,
        /// <summary>
        /// 
        /// </summary>
        Voice,
        /// <summary>
        /// 
        /// </summary>
        Fax,
        /// <summary>
        /// 
        /// </summary>
        Cell,
        /// <summary>
        /// 
        /// </summary>
        Video,
        /// <summary>
        /// 
        /// </summary>
        Pager,
        /// <summary>
        /// 
        /// </summary>
        Textphone,
        /// <summary>
        /// 
        /// </summary>
        MainNumber,
        /// <summary>
        /// 
        /// </summary>
        Pref
    }


    /// <summary>
    /// 
    /// </summary>
    public enum EmailType
    {
        /// <summary>
        /// 
        /// </summary>
        Internet,

        /// <summary>
        /// 
        /// </summary>
        Pref,

        /// <summary>
        /// 
        /// </summary>
        X400
    }


    /// <summary>
    /// 
    /// </summary>
    public enum VCardVersion
    {

        /// <summary>
        /// 
        /// </summary>
        VCard1,
        /// <summary>
        /// 
        /// </summary>
        VCard2,
        /// <summary>
        /// 
        /// </summary>
        VCard3,
        /// <summary>
        /// 
        /// </summary>
        VCard4,
    }

    /// <summary>
    /// 
    /// </summary>
    public enum RelationType
    {
        /// <summary>
        /// 
        /// </summary>
        Contact,
        /// <summary>
        /// 
        /// </summary>
        Acquaintance,
        /// <summary>
        /// 
        /// </summary>
        Friend,
        /// <summary>
        /// 
        /// </summary>
        Met,
        /// <summary>
        /// 
        /// </summary>
        CoWorker,
        /// <summary>
        /// 
        /// </summary>
        Colleague,
        /// <summary>
        /// 
        /// </summary>
        CoResiden,
        /// <summary>
        /// 
        /// </summary>
        Neighbor,
        /// <summary>
        /// 
        /// </summary>
        Child,
        /// <summary>
        /// 
        /// </summary>
        Parent,
        /// <summary>
        /// 
        /// </summary>
        Sibling,
        /// <summary>
        /// 
        /// </summary>
        Spouse,
        /// <summary>
        /// 
        /// </summary>
        Kin,
        /// <summary>
        /// 
        /// </summary>
        Muse,
        /// <summary>
        /// 
        /// </summary>
        Crush,
        /// <summary>
        /// 
        /// </summary>
        Date,
        /// <summary>
        /// 
        /// </summary>
        Sweetheart,
        /// <summary>
        /// 
        /// </summary>
        Me,
        /// <summary>
        /// 
        /// </summary>
        Agent,
        /// <summary>
        /// 
        /// </summary>
        Emergency,
    }
}
