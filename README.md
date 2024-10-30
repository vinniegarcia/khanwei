# Khanwei
Conway's Game of Life api in C#

## Running
1. `dotnet run -lp https`
2. Go to https://localhost:7246/swagger/
3. Try the endpoints out

## Testing
1. `dotnet test`
2. Test output should print to the terminal

## Compiling
1. `dotnet build`

## Decisions Made:
1. *Persistenace:* I went with SQLite for ease of setup. Yes this scales to just one machine. I could have gone with a distributed cache like Redis but that would add another layer of complexity and overhead, and I didn't think setting up a whole docker compose stack was worth it for an interview takehome exercise.
2. *ORM:* I could have used Entity Framework or Dapper, but it's SQLite and 2 SQL queries and nobody should be afraid to write some SQL :). 
3. *Serialization:* I used a 2D array of booleans for the game state, but there was a lot of serialization both into SQLite (because SQLite doesn't really have a JSON datatype so it's serialized to a string), and out into the HTTP response (where .NET wanted `List<List<bool>>` instead of `bool[,]`). There's probably a better way to do this than I did, but my C# and ASP.NET is rusty.
4. *Immutability:* I went with an "insert-once" approach here to save only initial board state, and any requests for future states are computed on demand. We can probably optimize this by caching/storing results of the calculations, but we don't need to scale that right now, extra compute usage is just fine.
5. *Idempotence:* Sending the same board config more than once for insert will not create duplicate records. It will just return the one existing in the DB. Check the returned guids to verify.
6. *Testing:* I used xunit, it was easy enough to get working. I wrote tests for the `BoardSerializer`, `ConwaysGameMove`, and `ConwayBoardService` classes. 
7. *Architecture:* I may not be following all the current best practices in .NET land, it's been about a decade since I did serious C# work and that's back when MVC was the new hotness. I used the minimal API, but that just means I ended up frankensteining my own framework. It's the Django/Flask dichotomy for C#. I injected a singleton service, but I could probably have done the `ConwayDatabase` -> `ConwayBoardService` linkage in a more idiomatic .NET way.

## Overall:
This took longer than expected, but I wanted to do a good job, and I had to do some catching up to get back into .NET. My day-to-day is TypeScript and Python, so C# felt familiar, but C# likes to pull in the OOP direction where I tend to go more functional (see the ConwaysGameMove class for me shoehorning the functional-programming "functions that operate on data structures" model into a class with a bunch of static methods so I can call them like loose functions). 

Anyway, I hope my explanations here make sense, and I hope they can help us get to the point more quickly during the interview with this context. If you've read this far, I applaud you, and if you actually pulled and ran my code, you probably deserve a medal.