using ListItAPI.Models;
using MongoDB.Bson;
using System.Collections.Generic;

namespace ListItDb.Data;

public class MongoDBListContext
{
    private readonly MongoDBHandler<Item> _itemHandler;

    string dbName = "ListItDb";
    string collectionName = "Items";
    
    
    public MongoDBListContext()
    {
        _itemHandler = new MongoDBHandler<Item>(dbName,collectionName);        
    }
    
    //Create new item in db
    public void Create(Item item)
    {
        _itemHandler.Create(item);
    } 
    //Get all items
    public List<Item> GetAll()
    {
        return _itemHandler.GetAll();
    }
    //Get items by value
    public List<Item> GetAllByValue(string value)
    {
        string Value = "";
        return _itemHandler.GetAll(Value, value);
    }
    //Get specific item
    public Item Get(ObjectId id)
    {
        return _itemHandler.Get(id);
    }
    //Update item by switching it for a new one
    public void Update(ObjectId id, Item item)
    {
        _itemHandler.Update(id, item);
    }
    //Delete all items in db
    public void DeleteAll()
    {
        _itemHandler.DeleteAll();
    }
    //Delete all items by value
    public void DeleteAllByValue(string value)
    {
        string Value = "";
        _itemHandler.DeleteAll(Value, value);
    }
    //Delete specific item
    public void Delete(ObjectId id)
    {
        _itemHandler.Delete(id);
    }   
   
}