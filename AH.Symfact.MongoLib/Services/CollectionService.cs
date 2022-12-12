using System;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using CommunityToolkit.Mvvm.Messaging;
using MongoDB.Bson;

namespace AH.Symfact.MongoLib.Services;

public class CollectionService
{
    private readonly ILogger _logger;

    public CollectionService(
        IMongoDbConnectionFactory connectionFactory,
        ILogger logger)
    {
        _logger = logger.ForContext<CollectionService>();
    }

    public async Task<int> InsertAsync(
        string xmlDataFile,
        Action<int> progress)
    {
        //var dataPath = WeakReferenceMessenger.Default.Send<DataFolderChangedMessage>();
        //var filePath = Path.Combine(dataPath, xmlDataFile);
        //_logger.Information("Starting to load from file '{FilePath}'...",
        //    filePath);

        //try
        //{
        //    var collection = database.GetCollection<BsonDocument>("NonXML");
        //    var xDoc = new XmlDocument();
        //    xDoc.Load(NonXMLDataSet);

        //    var parties = xDoc.SelectNodes("//nonXML", NamespaceManager);
        //    var index = 0;
        //    foreach (XmlNode party in parties)
        //    {
        //        var json = JsonConvert.SerializeXmlNode(party, new Newtonsoft.Json.Formatting(), true)
        //            .Replace("@ID", "_id");
        //        var bDoc = BsonDocument.Parse(json);
        //        collection.InsertOne(bDoc, new InsertOneOptions { });
        //        index++;
        //        Console.WriteLine($"{index}");
        //    }
        //    progress();

        //}
        //catch (Exception ex)
        {
            //_logger.Error(ex, "Reading {FilePath} failed", filePath);
            return 0;
        }
    }
}