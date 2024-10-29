using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IConwayBoardService, ConwayBoardService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var game = new ConwaysGame();

app.MapPost("/conway/board", (ConwayBoard b, IConwayBoardService conwayBoardService) => 
{
    return conwayBoardService.CreateBoard(b);
})
.WithName("AddBoard")
.WithOpenApi();

app.MapGet("/conway/board/{boardId}/next", (Guid boardId, IConwayBoardService conwayBoardService) =>
{
    var nextState = conwayBoardService.GetFutureState(boardId, 1);
    return nextState;
})
.WithName("GetNextBoardState")
.WithOpenApi();

app.MapGet("/conway/board/{boardId}/future/{ticks}", (Guid boardId, int ticks, IConwayBoardService conwayBoardService) =>
{
    var futureState = conwayBoardService.GetFutureState(boardId, ticks);
    return futureState;
})
.WithName("GetFutureBoardState")
.WithOpenApi();

app.MapGet("/conway/board/{boardId}/final/{maxAttempts}", (Guid boardId, int maxAttempts, IConwayBoardService conwayBoardService) =>
{
    var finalState = conwayBoardService.GetFinalState(boardId, maxAttempts);
    return finalState;
})
.WithName("GetFinalBoardState")
.WithOpenApi();

app.Run();
