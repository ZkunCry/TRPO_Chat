using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using System.Security.Cryptography;
using System.Text;
using WebApplication1;
using WebApplication1.ChatRoomService;
using WebApplication1.Hubs;
using WebApplication1.jwthandler;
using WebApplication1.middleware;
using WebApplication1.UserService;



internal class Program
{
    
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddSignalR();
        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddCors(opt =>
        {
            opt.AddPolicy(name:"chat-app", builder =>
            {
                builder.WithOrigins("http://localhost:3000").
                AllowAnyHeader().
                AllowAnyMethod();

            });
        });
        builder.Services.AddSingleton<IMongoDatabase>(provider =>
        {
            string connectStr = "mongodb://localhost:27017";
            var client = new MongoClient(connectStr);
            return client.GetDatabase("TRPOChat");

        });
        builder.Services.AddScoped<JwtToken>();
        builder.Services.AddSingleton<IChatRoomService, ChatRoomService>();

        builder.Services.AddSingleton<IUserService,UserService>();
       

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        app.UseCors("chat-app");

        app.UseMiddleware<JwtMiddleware>();

        app.UseAuthorization();

        app.MapControllers();
        app.MapHub<ChatHub>("/chat");

        app.Run();
    }
}