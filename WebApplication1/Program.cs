using MongoDB.Driver;
using System.Security.Cryptography;
using System.Text;
using WebApplication1;
using WebApplication1.ChatRoomService;
using WebApplication1.Hubs;
using WebApplication1.UserService;



internal class Program
{
    public static string ComputeSHA256Hash(string input)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] inputBytes = Encoding.UTF8.GetBytes(input);
            byte[] hashBytes = sha256.ComputeHash(inputBytes);

            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < hashBytes.Length; i++)
            {
                builder.Append(hashBytes[i].ToString("x2"));
            }

            return builder.ToString();
        }
    }
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
            opt.AddPolicy("chat-app", builder =>
            {
                builder.WithOrigins("http://localhost:3000").
                AllowAnyHeader().
                AllowAnyMethod().
                AllowCredentials();
            });
        });
        builder.Services.AddSingleton<IMongoDatabase>(provider =>
        {
            string connectStr = "mongodb://localhost:27017";
            var client = new MongoClient(connectStr);
            return client.GetDatabase("TRPOChat");

        });
        builder.Services.AddSingleton<IChatRoomService, ChatRoomService>();

        builder.Services.AddSingleton<IUserService,UserService>();
       

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseAuthorization();

        app.MapControllers();
        app.MapHub<ChatHub>("/chat");
        app.UseCors("chat-app");
        app.Run();
    }
}