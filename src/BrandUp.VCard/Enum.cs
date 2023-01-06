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
        VALUE,
        PREF,
        ALTID,
        PID,
        TYPE,
        MEDIATYPE,
        CALSCALE,
        SORTAS,
        GEO,
        TZ
    }

    public enum CardProperty
    {
        SOURCE,
        KIND,
        XML,
        FN,
        N,
        NICKNAME,
        PHOTO,
        BDAY,
        ANNIVERSARY,
        GENDER,
        ADR,
        TEL,
        EMAIL,
        IMPP,
        LANG,
        TZ,
        GEO,
        TITLE,
        ROLE,
        LOGO,
        ORG,
        MEMBER,
        RELATED,
        CATEGORIES,
        NOTE,
        PRODID,
        REV,
        SOUND,
        UID,
        CLIENTPIDMAP,
        URL,
        VERSION,
        KEY,
        FBURL,
        CALADRURI,
        CALURI
    }

    public enum Kind
    {
        Work,
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
