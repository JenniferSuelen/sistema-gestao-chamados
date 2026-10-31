using Api.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException(
        "A connection string 'DefaultConnection' não foi configurada."
    );
var sqliteConnectionString = new SqliteConnectionStringBuilder(connectionString);

if (!string.IsNullOrWhiteSpace(sqliteConnectionString.DataSource) &&
    !string.Equals(sqliteConnectionString.DataSource, ":memory:", StringComparison.Ordinal) &&
    !sqliteConnectionString.DataSource.StartsWith("file:", StringComparison.OrdinalIgnoreCase) &&
    !Path.IsPathRooted(sqliteConnectionString.DataSource))
{
    sqliteConnectionString.DataSource = Path.GetFullPath(
        Path.Combine(
            builder.Environment.ContentRootPath,
            sqliteConnectionString.DataSource
        )
    );
}

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(sqliteConnectionString.ToString()));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();

public partial class Program
{
}