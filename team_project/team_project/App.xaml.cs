using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;

namespace team_project
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        static App()
        {
            FrameworkElement.LanguageProperty.OverrideMetadata(
                typeof(FrameworkElement),
                new FrameworkPropertyMetadata(
                    XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));
        }
        private void ButtonMouseEnter(object sender, MouseEventArgs e)
        {
            var brush = Application.Current.Resources["ButtonMouseEnter"] as SolidColorBrush;
            ((Border)sender).Background = brush;
        }

        private void ButtonMouseLeave(object sender, MouseEventArgs e)
        {
            var brush = Application.Current.Resources["ButtonMouseLeave"] as SolidColorBrush;
            ((Border)sender).Background = brush;
        }

        private void ButtonBackgound_MouseEnter(object sender, MouseEventArgs e)
        {
            var brush = Application.Current.Resources["ButtonOverlayMouseEnter"] as SolidColorBrush;
            ((Border)sender).Background = brush;
        }

        private void ButtonBackgound_MouseLeave(object sender, MouseEventArgs e)
        {
            var brush = Application.Current.Resources["ButtonOverlayMouseLeave"] as SolidColorBrush;
            ((Border)sender).Background = brush;
        }
        private void ButtonMouseCancelEnter(object sender, MouseEventArgs e)
        {
            ((Border)sender).Background = new SolidColorBrush(Color.FromArgb(255, 130, 130, 130));
        }

        private void ButtonMouseCancelLeave(object sender, MouseEventArgs e)
        {
            ((Border)sender).Background = new SolidColorBrush(Color.FromArgb(255, 88, 88, 88));
        }

        private void ButtonMouseExitEnter(object sender, MouseEventArgs e)
        {
            ((Border)sender).Background = new SolidColorBrush(Color.FromArgb(255, 209, 82, 40));
        }

        private void ButtonMouseExitLeave(object sender, MouseEventArgs e)
        {
            ((Border)sender).Background = new SolidColorBrush(Color.FromArgb(255, 189, 47, 0));
        }

        private void ButtonMouseSendEnter(object sender, MouseEventArgs e)
        {
            ((Border)sender).Background = new SolidColorBrush(Color.FromArgb(255, 128, 227, 93));
        }

        private void ButtonMouseSendLeave(object sender, MouseEventArgs e)
        {
            ((Border)sender).Background = new SolidColorBrush(Color.FromArgb(255, 58, 172, 18));
        }

        private void ListView_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (!e.Handled)
            {
                e.Handled = true;
                var eventArg = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta);
                eventArg.RoutedEvent = UIElement.MouseWheelEvent;
                eventArg.Source = sender;
                var parent = ((Control)sender).Parent as UIElement;
                parent.RaiseEvent(eventArg);
            }
        }
    }
}
