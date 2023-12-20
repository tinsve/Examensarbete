using ListItClient.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
// CORS options for policy
builder.Services.AddCors(options =>
{
options.AddPolicy("AllowAllOrigins", builder =>
{
builder.AllowAnyOrigin()
.AllowAnyHeader()
.AllowAnyMethod();
});
});builder.Services.AddHttpClient<ItemsApiService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
// CORS policy
app.UseCors("AllowAllOrigins");
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
