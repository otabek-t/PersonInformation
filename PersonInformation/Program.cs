using Microsoft.Extensions.DependencyInjection;
using MyFirstEBot;
using PersonInformation.Bll.Services;

namespace PersonInformation
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddScoped<IUserService, UserService>();
            serviceCollection.AddSingleton<MainContext>();
            serviceCollection.AddSingleton<TelegramBotListener>();


            var serviceProvider = serviceCollection.BuildServiceProvider();

            var botListenerService = serviceProvider.GetRequiredService<TelegramBotListener>();
            await botListenerService.StartBot();

            Console.ReadKey();
        }
    }
}
