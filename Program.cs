using System.Collections.Generic;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var game = new ConwaysGame();

app.MapPost("/conway/board", (ConwayBoard b) => 
{
    var boardId = game.UploadBoard(BoardSerializer.FromSerializableFormat(b.board));
    return boardId;
})
.WithName("AddBoard")
.WithOpenApi();

app.MapGet("/conway/board/{boardId}/next", (Guid boardId) =>
{
    var nextState = game.GetNextState(boardId);
    return BoardSerializer.ToSerializableFormat(nextState);
})
.WithName("GetNextBoardState")
.WithOpenApi();

app.MapGet("/conway/board/{boardId}/future/{ticks}", (Guid boardId, int ticks) =>
{
    var futureState = game.GetFutureState(boardId, ticks);
    return BoardSerializer.ToSerializableFormat(futureState);
})
.WithName("GetFutureBoardState")
.WithOpenApi();

app.MapGet("/conway/board/{boardId}/final/{maxAttempts}", (Guid boardId, int maxAttempts = 10) =>
{
    var finalState = game.GetFinalState(boardId, maxAttempts);
    return BoardSerializer.ToSerializableFormat(finalState);
})
.WithName("GetFinalBoardState")
.WithOpenApi();

app.Run();

public record ConwayBoard {  
    public List<List<bool>> board { get; set; }
}