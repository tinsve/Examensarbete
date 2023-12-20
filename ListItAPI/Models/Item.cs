using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace ListItAPI.Models;
public class Item
{    
    public ObjectId Id { get; set; }

    public bool Done {  get; set; }
       
    public int? Amount { get; set; }

    public string? Name { get; set; }

    public string? Type { get; set; }    
    
}