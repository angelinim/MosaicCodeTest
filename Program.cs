using Mosaic2.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the Dependancy Injection container.
{
    var services = builder.Services;
    var env = builder.Environment;

    services.AddControllers();
    services.AddEndpointsApiExplorer();
    services.AddSwaggerGen();

    //I would normally not use a singleton for a service
    //but in order for data to persist I have to make the
    //service persist
    services.AddSingleton<IRepository, Repository>();
}

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

app.Run();
