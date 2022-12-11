using Microsoft.EntityFrameworkCore;

// ConfigureService
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Definindo Contexto como um serviço
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseInMemoryDatabase("TarefasDB"));

// -- Configure --
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}



// Definindo endpoint
app.MapGet("/", () => "Olá Mundo");

app.MapGet("/tarefas", async (AppDbContext db) => {
    var listaTarefa = await db.Tarefas.ToListAsync();
    return listaTarefa;
});

app.MapGet("/tarefas/{id:int}", async (int id, AppDbContext db) =>
    await db.Tarefas.FindAsync(id) is Tarefa tarefa ? Results.Ok(tarefa) : Results.NotFound());
/*app.MapGet("/tarefas/{id:int}", async (int id, AppDbContext db) =>{
    var existeTarefa = await db.Tarefas.FindAsync(id);

    if (existeTarefa != null)
    {
        return Results.Ok(existeTarefa);
    }
    else
    {
        return Results.NotFound("Tarefa não encontrada!");
    }
});*/

app.MapGet("/tarefas/concluida", async (AppDbContext db) =>
    await db.Tarefas.Where(t => t.IsConcluida).ToListAsync());

app.MapPost("/tarefas", async (Tarefa tarefa, AppDbContext db) =>{
    db.Tarefas.Add(tarefa);
    await db.SaveChangesAsync();
    return Results.Created($"/tarefas/{tarefa.Id}", tarefa);
});

app.MapPut("/tarefas/{id:int}", async (int id, AppDbContext db, Tarefa inputTarefa) => {
    var tarefa = await db.Tarefas.FindAsync(id);

    if (tarefa is null) return Results.NotFound("Tarefa não encontrada!");

    tarefa.Nome = inputTarefa.Nome;
    tarefa.IsConcluida = inputTarefa.IsConcluida;

    await db.SaveChangesAsync();

    return Results.NoContent();
});

app.MapDelete("/tarefas/{id:int}", async (int id, AppDbContext db) =>
{
    if(await db.Tarefas.FindAsync(id) is Tarefa tarefa)
    {
        db.Tarefas.Remove(tarefa);
        await db.SaveChangesAsync();
        return Results.Ok();
    }

    return Results.NotFound();
});


// Consumindo outra api
app.MapGet("frases", async () =>
    await new HttpClient().GetStringAsync("http://ron-swanson-quotes.herokuapp.com/v2/quotes")
);


app.Run();

// Modelo
class Tarefa
{
    public int Id { get; set; }
    public string? Nome { get; set; }
    public bool IsConcluida { get; set; }
}

// Contexto
class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Tarefa> Tarefas => Set<Tarefa>();
}
