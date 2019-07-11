using System;
using System.Windows;

namespace VlcCrashRepro
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            DispatcherUnhandledException += (sender, args) =>
            {
                Console.WriteLine($"An exception occurred: {args.Exception}");
            };
            AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
            {
                Console.WriteLine($"An exception occurred: {args.ExceptionObject}");
            };
            Exit += (sender, args) =>
            {
                Console.WriteLine("application exits");
            };
        }
    }
}
