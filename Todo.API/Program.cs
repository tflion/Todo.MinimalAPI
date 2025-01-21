using Microsoft.EntityFrameworkCore;
using Todo.API;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<TodoDb>(opt => opt.UseInMemoryDatabase("TodoList"));

var app = builder.Build();

app.MapGet("/todo-items", async (TodoDb db) => 
    await db.Todos.ToListAsync());

app.MapGet("/todo-items/{id}", async (int id, TodoDb db) =>
    await db.Todos.FindAsync(id));

app.MapPost("/todo-items", async (TodoItem todo, TodoDb db) =>
{
    db.Todos.Add(todo);
    await db.SaveChangesAsync();
    return Results.Created($"/todo-items/{todo.Id}", todo);
});

app.MapPut("/todo-items/{id}", async (int id, TodoItem input, TodoDb db) =>
{
    var todoItem = await db.Todos.FindAsync(id);
    if(todoItem == null) return Results.NotFound();
    todoItem.Name = input.Name;
    todoItem.IsComplete = input.IsComplete;
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.MapDelete("/todo-items/{id}", async (int id, TodoDb db) =>
{
    if(await db.Todos.FindAsync(id) is TodoItem todo)
    {
        db.Todos.Remove(todo);
        await db.SaveChangesAsync();
        return Results.NoContent() ;
    }

    return Results.NotFound();
});

app.Run();
