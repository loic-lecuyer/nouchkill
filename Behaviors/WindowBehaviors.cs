using Avalonia;
using Avalonia.Controls;
using System;
using System.Windows.Input;

namespace NouchKill.Behaviors
{
    public static class WindowBehaviors
    {
        public static readonly AttachedProperty<ICommand> OpenedCommandProperty =
            AvaloniaProperty.RegisterAttached<Window, ICommand>(
                "OpenedCommand", typeof(WindowBehaviors));

        public static void SetOpenedCommand(Window window, ICommand value) =>
            window.SetValue(OpenedCommandProperty, value);

        public static ICommand GetOpenedCommand(Window window) =>
            window.GetValue(OpenedCommandProperty);

        static WindowBehaviors()
        {
            OpenedCommandProperty.Changed.Subscribe(args =>
            {
                if (args.Sender is Window window)
                {
                    window.Opened -= Window_Opened;
                    if (args.NewValue.Value is ICommand)
                    {
                        window.Opened += Window_Opened;
                    }
                }
            });
            ClosingCommandProperty.Changed.Subscribe(args =>
            {
                if (args.Sender is Window window)
                {
                    AttachClosingHandler(window);
                }
            });
        }

        private static void Window_Opened(object? sender, EventArgs e)
        {
            if (sender is Window window)
            {
                var command = GetOpenedCommand(window);
                if (command?.CanExecute(null) == true)
                {
                    command.Execute(null);
                }
            }
        }

        public static readonly AttachedProperty<ICommand> ClosingCommandProperty =
            AvaloniaProperty.RegisterAttached<Window, ICommand>(
                "ClosingCommand", typeof(WindowBehaviors));

        public static void SetClosingCommand(Window window, ICommand value) =>
            window.SetValue(ClosingCommandProperty, value);

        public static ICommand GetClosingCommand(Window window) =>
            window.GetValue(ClosingCommandProperty);

        static void AttachClosingHandler(Window window)
        {
            window.Closing -= Window_Closing;
            window.Closing += Window_Closing;
        }


        private static void Window_Closing(object? sender, WindowClosingEventArgs e)
        {
            if (sender is Window window)
            {
                var command = GetClosingCommand(window);
                if (command?.CanExecute(null) == true)
                {
                    command.Execute(null);
                }
            }
        }
    }
}
