
using OperationalTransformation;

namespace SignalR
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddSignalR();

            var syncSystem = new SynchronizationSystem();
            builder.Services.AddSingleton(syncSystem);

            var app = builder.Build();

            app.UseHttpsRedirection();

            app.MapHub<DocumentsHub>("/documents");

            app.Run();
        }
    }
}
