var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Defininco endpoint
app.MapGet("/", () => "Olá Mundo");

app.MapGet("frases", async () =>
    await new HttpClient().GetStringAsync("http://ron-swanson-quotes.herokuapp.com/v2/quotes")
);

app.Run();