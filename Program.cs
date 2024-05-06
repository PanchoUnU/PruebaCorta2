using System.Linq.Expressions;
using chairs_dotnet7_api;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<DataContext>(opt => opt.UseInMemoryDatabase("chairlist"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

var app = builder.Build();
var chairs = app.MapGroup("api/chair");
var CreateSilla = app.MapGroup("api/chair");
var ObtenerSilas = app.MapGroup("api/chair");
var obtenerSillaPorNombre = app.MapGroup("api/chair/{nombre}");
var ActualizarSilla1 = app.MapGroup("api/chair{Id}");
var IncrementaStock = app.MapGroup("api/chair/{Id}/Stock");
var CompraSilla = app.MapGroup("api/chair/purchase");
var eliminarSilla = app.MapGroup("api/chair/{Id}");

//TODO: ASIGNACION DE RUTAS A LOS ENDPOINTS
chairs.MapGet("/", GetChairs);
CreateSilla.MapPost("/",PotSillas);
ObtenerSilas.MapGet("/",GetChairAll);
obtenerSillaPorNombre.MapGet("/",GetSillaNombre);
ActualizarSilla1.MapPut("/",ActualizarSilla);
IncrementaStock.MapPost("/",PostStock);
CompraSilla.MapPost("/",postCompra);
eliminarSilla.MapDelete("/",DeleteSilla);
app.Run();

//TODO: ENDPOINTS SOLICITADOS
static IResult GetChairs(DataContext db)
{
    return TypedResults.Ok();
}
static IResult PotSillas(DataContext db, Chair chair)
{
    var chairs =  db.Chairs.Find(chair.Nombre);
    if(chair is not null)
    {
        db.Chairs.Add(chairs);
        db.SaveChanges();
        return TypedResults.Created($"/Chair/{chairs}",chairs);

    }
    return TypedResults.BadRequest("La silla ya se encuentra");
}
static IResult GetChairAll(DataContext db)
{
    return TypedResults.Ok(db.Chairs.Where(u => u.Stock >0).ToListAsync());
}
static IResult GetSillaNombre(DataContext db, string nombre1)
{
    
    var chairs = db.Chairs.Find(nombre1);
    if(chairs is not null)
    {
        return TypedResults.Ok(chairs);
    }
    return TypedResults.BadRequest("No se encontro la silla");
}

static IResult ActualizarSilla(DataContext db, Chair chair)
{
    var silla = db.Chairs.Find(chair.Id);
    if(silla is not null)
    {
        silla.Nombre = chair.Nombre;
        silla.Tipo = chair.Tipo;
        silla.Altura = chair.Altura;
        silla.Anchura = chair.Anchura;
        silla.Color = chair.Color;
        silla.Id = chair.Id;
        silla.Material = chair.Material;
        silla.Precio = chair.Precio;
        silla.Profundidad =chair.Profundidad;

        db.SaveChanges();
        return TypedResults.NoContent();
    }
    return TypedResults.NotFound();

}
static IResult PostStock(DataContext db, Chair chair)
{
    var silla = db.Chairs.Find(chair.Id);
    if(silla is null)
    {
        return TypedResults.NotFound();
    }
    silla.Stock = chair.Stock;
    db.SaveChanges();
    return TypedResults.Ok("Se incremento exitosamente el stock de las sillas");

}

static IResult postCompra(DataContext db, int id, int cantidad, int pago)
{
    var silla = db.Chairs.Find(id);
    if(silla is null)
    {
        return TypedResults.BadRequest("No se encotro la silla");
    }
    if (silla.Stock < cantidad || silla.Precio >pago)
    {
        return TypedResults.BadRequest("el pago o la cantidad es insuficiente.");
    }
    silla.Stock = silla.Stock - cantidad;
    db.SaveChanges();
    return TypedResults.Ok("Se logro realizar la compra");
}

static IResult DeleteSilla(DataContext db, int id)
{
    var silla = db.Chairs.Find(id);
    if(silla is null)
    {
        return TypedResults.BadRequest("No se encotro la silla");
    }
    db.Chairs.Remove(silla);
    db.SaveChanges();
    return TypedResults.NoContent();
}



