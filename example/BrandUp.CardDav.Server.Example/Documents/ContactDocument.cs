﻿using BrandUp.CardDav.Server.Documents;
using BrandUp.MongoDB;
using MongoDB.Bson.Serialization.Attributes;

namespace BrandUp.CardDav.Server.Example.Documents
{
    [Document(CollectionName = "CardDav.Contacts")]
    public class ContactDocument : IContactDocument
    {
        [BsonId]
        public Guid Id { get; set; }
        public Guid AddressBookId { get; set; }
        public string VCard { get; set; }
    }
}