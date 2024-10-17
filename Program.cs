using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using ToDoAPI;
using ToDoAPI.Domain;
using ToDoAPI.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAWSLambdaHosting(LambdaEventSource.HttpApi);

string connectionString = builder.Configuration.GetConnectionString("ToDoDB");

builder.Services.AddDbContext<ToDoContext>(options => options.UseSqlServer(connectionString));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var toDoContext = services.GetRequiredService<ToDoContext>();
    toDoContext.Database.EnsureCreated();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/ToDo/{id}", async (int id, ToDoContext toDoContext) =>
{
    var toDo = await toDoContext.ToDos.SingleOrDefaultAsync(x => x.Id == id);

    if (toDo == null)
        return Results.NotFound();

    return Results.Ok(toDo);
})
.WithName("GetToDo")
.WithOpenApi();

app.MapDelete("/ToDo/{id}", async (int? id, ToDoContext toDoContext) =>
{
    if (!id.HasValue)
        return Results.BadRequest("ToDo Id cannot be empty");

    var toDoToBeDeleted = await toDoContext.ToDos.SingleOrDefaultAsync(x => x.Id == id);

    if (toDoToBeDeleted == null)
        return Results.NotFound();

    toDoContext.Remove(toDoToBeDeleted);
    
    await toDoContext.SaveChangesAsync();

    return Results.NoContent();
})
.WithName("DeleteToDo")
.WithOpenApi();

app.MapGet("/ToDo/", async (ToDoContext toDoContext) =>
{
    var toDos = await toDoContext.ToDos.ToListAsync();

    return Results.Ok(toDos);
})
.WithName("GetToDos")
.WithOpenApi();

app.MapPost("/ToDo/", async (ToDoInput input, ToDoContext toDoContext) =>
{
    if (input.Name.IsNullOrEmpty())
        return Results.BadRequest("ToDo name cannot be empty");

    var newToDo = new ToDo(input.Name);

    var createdToDo = await toDoContext.AddAsync(newToDo);
    await toDoContext.SaveChangesAsync();

    return Results.Created($"/ToDo/{createdToDo.Entity.Id}", createdToDo.Entity);
})
.WithName("CreateToDo")
.WithOpenApi();


app.MapPut("/ToDo/{id}", async (int? id, ToDoInput input, ToDoContext toDoContext) =>
{
    if (input.Name.IsNullOrEmpty())
        return Results.BadRequest("ToDo name cannot be empty");

    if (!id.HasValue)
        return Results.BadRequest("ToDo Id cannot be empty");

    var existingToDo = await toDoContext.ToDos.SingleOrDefaultAsync(x =>x.Id == id);    

    if (existingToDo == null) 
        return Results.NotFound();

    existingToDo.Name = input.Name;

    if (input.Complete == true)
        existingToDo.CompleteToDo();

    await toDoContext.SaveChangesAsync();

    return Results.Created($"/ToDo/{existingToDo.Id}", existingToDo);
})
.WithName("UpdateToDo")
.WithOpenApi();

app.Run();

