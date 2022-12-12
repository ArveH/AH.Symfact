﻿using MongoDB.Driver;

namespace AH.Symfact.UI.MongoDb;

public class MongoDbConnectionFactory : IMongoDbConnectionFactory
{
    public MongoDbConnectionFactory(MongoDbConnectionString mongoDbConnectionString)
    {
        MongoDbConnectionString = mongoDbConnectionString;
    }

    public MongoDbConnectionString MongoDbConnectionString { get; }

    public IMongoClient CreateClient()
    {
        return new MongoClient(MongoDbConnectionString.ConnectionString);
    }

    public IMongoDatabase GetDatabase()
    {
        return CreateClient().GetDatabase(MongoDbConnectionString.DatabaseName);
    }
}