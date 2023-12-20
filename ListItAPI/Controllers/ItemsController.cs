using Microsoft.AspNetCore.Mvc;
using ListItAPI.Models;
using System;

using Microsoft.AspNetCore.Http.HttpResults;
using System.IO;
using System.Reflection;
using System.Text.Json.Serialization;
using System.Text.Json;
using ListItDb.Data;
using MongoDB.Bson;
namespace ListItAPI.Controllers;
[ApiController]
[Route("api/[controller]")]
public class ItemsController : ControllerBase
{
    private static MongoDBListContext context = new MongoDBListContext(); 
           
    [HttpGet]
    public IEnumerable<Item> Get()
    {       
        var items= context.GetAll();       
        return items;
    }

   
    [HttpGet("{id}")]
    public Item? Get(ObjectId id)
    {
        var item = context.Get(id);
        return item;
    }
   
    [HttpPost]
    public void Post([FromBody] Item item)
    {
        context.Create(item);
    }
   
    [HttpPut("{id}")]
    public async Task Put(ObjectId id, [FromBody] Item newItem)
    {
        var item = new Item();
        item.Id = id;
        item.Done = newItem.Done;
        item.Amount = newItem.Amount;
        item.Name = newItem.Name;
        item.Type = newItem.Type;
        context.Update(id, item);
    }
   
    [HttpDelete("{id}")]
    public async Task Delete(ObjectId id)
    {
        context.Delete(id);
    }
}
