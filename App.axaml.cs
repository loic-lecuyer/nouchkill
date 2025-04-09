using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using NouchKill.Services;
using NouchKill.ViewModels;
using NouchKill.Views;

namespace NouchKill
{
    public static class ServiceCollectionExtensions {
        public static void AddCommonServices(this IServiceCollection collection) {
          //  collection.AddSingleton<IRepository, Repository>();
           // collection.AddTransient<BusinessService>();
            collection.AddTransient<MainWindowViewModel>();
            collection.AddTransient<SettingViewModel>();
            collection.AddSingleton<SettingService>();
        }
    }
    public partial class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            // Register all the services needed for the application to run
            var collection = new ServiceCollection();
            collection.AddCommonServices();

            // Creates a ServiceProvider containing services from the provided IServiceCollection
            var services = collection.BuildServiceProvider();

            var vm = services.GetRequiredService<MainWindowViewModel>();
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop) {
                desktop.MainWindow = new MainWindow {
                    DataContext = vm
                };
            } else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform) {
                singleViewPlatform.MainView = new MainWindow {
                    DataContext = vm
                };
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}