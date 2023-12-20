using System.Text.Json;
using System.Text;
using MongoDB.Bson;
using ListItClient.Models;
namespace ListItClient.Services;
public class ItemsApiService
{
    private readonly HttpClient _httpClient;
    
    private string _baseUrl = "http://localhost:5169/api/items";
    public ItemsApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    
    public async Task<List<ItemApiDTO>> GetItemsAsync()
    {
        var response = await _httpClient.GetAsync(_baseUrl);
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<List<ItemApiDTO>>(content);
    }
    public async Task<ItemApiDTO>GetItemByIdAsync(ObjectId id)
    {
        var response = await _httpClient.GetAsync($"{_baseUrl}?Id={id}");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<ItemApiDTO>(content);
    }
    public async Task CreateItemAsync(int amount, string name, string type)
    {
        var item = new ItemApiDTO();
        item.Done = false;
        item.Amount = amount;
        item.Name = name;
        item.Type = type;

       
        var itemToAdd = JsonSerializer.Serialize(item);
        var requestContent = new StringContent(itemToAdd, Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync(_baseUrl, requestContent);
        response.EnsureSuccessStatusCode();       
       
    }
   public async Task UpdateItemAsync(ObjectId id,ItemsViewModel item)
    {        
        var updatedItem=JsonSerializer.Serialize(item);
        var requestContent = new StringContent(updatedItem, Encoding.UTF8, "application/json");
        var uri = Path.Combine(_baseUrl + $"/{id}");
        await _httpClient.PutAsync(_baseUrl + $"/{id}", requestContent);
    }
    public async Task ToggleStatusAsync(ObjectId id)
    {
        var response = await _httpClient.GetAsync(_baseUrl+ $"/{id}");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        var item=JsonSerializer.Deserialize<ItemApiDTO>(content);
        var updatedItem=new ItemApiDTO();
        updatedItem.Id = item.Id;
        if(item.Done==true) { updatedItem.Done = false; }
        else if (item.Done == false) { updatedItem.Done = true; }
        var newItem = JsonSerializer.Serialize(updatedItem);
        var requestContent = new StringContent(newItem, Encoding.UTF8, "application/json");
        var uri = Path.Combine(_baseUrl + $"/{id}");
        await _httpClient.PutAsync(uri, requestContent);       
    }
        
    public async Task DeleteAsync(ObjectId id)
    {        
        await _httpClient.DeleteAsync(_baseUrl + $"/{id}");               
    }
    public async Task DeleteDoneAsync()
    {
       var items=await _httpClient.GetFromJsonAsync<List<ItemApiDTO>>($"{_baseUrl}?Done=true");        
       foreach(var item in items)
        { 
           await _httpClient.DeleteAsync(_baseUrl+$"/{item.Id}");            
        }
    }
}