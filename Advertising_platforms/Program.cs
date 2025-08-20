var builder = WebApplication.CreateBuilder(args);

// ✅ Подключаем Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(80); // в Docker
});

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(); 
    
}

app.UseHttpsRedirection();
app.MapControllers();



// app.MapGet("/testtttt", () =>
//     {
//         Console.WriteLine("test here");
//         
//         return Results.Ok("Test");
//     })
//     .WithName("testt");

app.Run();

