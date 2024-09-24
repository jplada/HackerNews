using HackerNews.Services.Interfaces;
using HackerNews.Services.Services;

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
        policy =>
        {
            policy.WithOrigins("*");
        });
});

// Add services to the container.
builder.Services.AddMemoryCache();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient<IHackerNewsServiceAgent, HackerNewsServiceAgent>();
builder.Services.AddSingleton<ICacheService, CacheService>();
builder.Services.AddScoped<IHackerNewsServiceAgent, HackerNewsServiceAgent>();
builder.Services.AddScoped<INewsService, NewsService>();
var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
if (app.Environment.IsDevelopment())
{    
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.UseCors(MyAllowSpecificOrigins);

using (var serviceScope = app.Services.CreateScope())
{
    var services = serviceScope.ServiceProvider;
    var newsService = services.GetRequiredService<INewsService>();
    await newsService.LoadLatestNewsInCache();
}
app.Run();
