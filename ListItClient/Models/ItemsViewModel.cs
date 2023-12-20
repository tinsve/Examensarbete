using MongoDB.Bson;

namespace ListItClient.Models;
public class ItemsViewModel
{
public ObjectId Id { get; set; }

public bool Done {  get; set; }
       
public int? Amount { get; set; }

public string? Name { get; set; }

public string? Type { get; set; }
}