using KoeHandel.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<KoeHandelContext>(optionsBuilder =>
  optionsBuilder
  .UseInMemoryDatabase("KoeHandelDb", options => options.EnableNullChecks())
  .UseSeeding((context, _) => context.Seed())
  .UseAsyncSeeding((context, _, cancellationToken) => context.SeedAsync(cancellationToken))
 );
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<KoeHandelContext>();
    await context.Database.EnsureCreatedAsync();
}

await app.RunAsync();
