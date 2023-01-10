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

    public enum TelType
    {
        Text,
        Voice,
        Fax,
        Cell,
        Video,
        Pager,
        Textphone,
        MainNumber,
        Pref
    }

    public enum EmailType
    {
        Internet,
        Pref,
        X400
    }

    public enum VCardVersion
    {
        VCard1,
        VCard2,
        VCard3,
        VCard4,
    }
    public enum RelationType
    {
        Contact,
        Acquaintance,
        Friend,
        Met,
        CoWorker,
        Colleague,
        CoResiden,
        Neighbor,
        Child,
        Parent,
        Sibling,
        Spouse,
        Kin,
        Muse,
        Crush,
        Date,
        Sweetheart,
        Me,
        Agent,
        Emergency,
    }
}
