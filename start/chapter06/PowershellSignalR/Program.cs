using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddDbContext<VoteDbContext>(options =>
        options.UseSqlite("Data Source=./Data/Data.db")
);

builder.Services.AddSignalR();

var app = builder.Build();

app.UseStaticFiles();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapHub<VotingHub>("/votingHub");
app.MapControllers();

app.MapFallbackToFile("index.html");

app.Run();
