using FullStack.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<FullStackDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("CivicIDDB")));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<FullStackDbContext>();
    Console.WriteLine($"Citizens count: {context.Citizens.Count()}");
    Console.WriteLine($"Documents count: {context.Documents.Count()}");
}
