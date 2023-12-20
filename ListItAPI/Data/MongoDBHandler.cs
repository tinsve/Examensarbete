using MongoDB.Bson;
using MongoDB.Driver;

namespace ListItDb.Data;
public class MongoDBHandler<T>
{
       
    private readonly IMongoCollection<T> _collection;
    private readonly IMongoDatabase _database;
  
    public MongoDBHandler(string databaseName, string collectionName)
    {


        var connectionString = "mongodb://mongodb-service:27017";        
        var client = new MongoClient(connectionString);
        _database = client.GetDatabase(databaseName);
        var collections = _database.ListCollections().ToList();
        if (!collections.Any(c => c.GetValue("name").ToString() == collectionName))
        {
            _database.CreateCollection(collectionName);
        }
        _collection = _database.GetCollection<T>(collectionName);
    }
    
    //CRUD
    public void Create(T obj)
    {
        _collection.InsertOne(obj);
    }    
    public T Get(ObjectId id)
    {
        var filter = Builders<T>.Filter.Eq("Id", id);
        return _collection.Find(filter).FirstOrDefault();
    }
    public T GetByValue(string key, string value)
    {
        var filter = Builders<T>.Filter.Eq(key, value);
        return _collection.Find(filter).FirstOrDefault();
    }
    public List<T> GetAll()
    {
        return _collection.Find(new BsonDocument()).ToList();
    }
    public List<T> GetAll(string key, string value)
    {
        var filter = Builders<T>.Filter.Eq(key, value);
        return _collection.Find(filter).ToList();
    }
    public void Update(ObjectId id, T obj)
    {
        var filter = Builders<T>.Filter.Eq("Id", id);
        _ = _collection.ReplaceOne(filter, obj);
    }    
    public void Delete(ObjectId id)
    {
        var filter = Builders<T>.Filter.Eq("Id", id);
        _ = _collection.DeleteOne(filter);
    }
    internal void DeleteAll()
    {
        _ = _collection.DeleteMany(new BsonDocument());
    }
    internal void DeleteAll(string key, string value)
    {
        var filter = Builders<T>.Filter.Eq(key, value);
        _ = _collection.DeleteMany(filter);
    }
    
}