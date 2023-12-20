using ListItClient.Models;
using ListItClient.Services;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
namespace ListItClient.Controllers;
public class ItemsController : Controller
{
    private readonly ItemsApiService _itemsApiService;
    public ItemsController(ItemsApiService itemsApiService)
    {
        _itemsApiService = itemsApiService;
    }
    [HttpGet]
    public async Task<IActionResult> Index()
    {
       
        var itemsFromApiService = await _itemsApiService.GetItemsAsync();
       
        var items = itemsFromApiService.Select(p => new ItemsViewModel
        {
            Done = p.Done,
            Amount = p.Amount,
            Name = p.Name,
            Type = p.Type
        }
        );
      
        return View(items);
    }
    [HttpPost]
    public async Task<IActionResult> Create(int amount, string name, string type)
    {
        await _itemsApiService.CreateItemAsync(amount, name, type);

        return RedirectToAction("Index");
    }
    [HttpPost]
    public async Task<IActionResult> ToggleStatus(ObjectId id)
    {
        var item=await _itemsApiService.GetItemByIdAsync(id);
        var updatedItem = new ItemsViewModel();
        updatedItem.Id = id;
        if (item.Done == true) { updatedItem.Done = false; }
        else if (item.Done == false) { updatedItem.Done = true; }
        updatedItem.Amount = item.Amount;
        updatedItem.Name = item.Name;
        updatedItem.Type = item.Type;             
        
        await _itemsApiService.UpdateItemAsync(id, updatedItem);

        return RedirectToAction("Index");
    }
    
    [HttpPost]
    public async Task<IActionResult> Delete(ObjectId id) 
    {
        await _itemsApiService.DeleteAsync(id);
        return RedirectToAction("Index");
    }
    [HttpPost]
    public async Task<IActionResult> DeleteDone()
    {
        await _itemsApiService.DeleteDoneAsync();        
        return RedirectToAction("Index");
    }
}