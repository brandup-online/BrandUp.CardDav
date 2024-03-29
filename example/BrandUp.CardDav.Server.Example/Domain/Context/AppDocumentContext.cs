﻿using BrandUp.CardDav.Server.Example.Domain.Documents;
using BrandUp.MongoDB;
using MongoDB.Driver;

namespace BrandUp.CardDav.Server.Example.Domain.Context
{
    public class AppDocumentContext : MongoDbContext
    {
        public AppDocumentContext(MongoDbContextOptions options) : base(options)
        {
        }

        public IMongoCollection<UserDocument> Users => GetCollection<UserDocument>();

        public IMongoCollection<AddressBookDocument> AddressBooks => GetCollection<AddressBookDocument>();

        public IMongoCollection<ContactDocument> Contacts => GetCollection<ContactDocument>();
    }
}
