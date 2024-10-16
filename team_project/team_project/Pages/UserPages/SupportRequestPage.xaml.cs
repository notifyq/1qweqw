using Notification.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using team_project.Api;
using team_project.Model;

namespace team_project.Pages.UserPages
{
    /// <summary>
    /// Логика взаимодействия для SupportRequestPage.xaml
    /// </summary>
    public partial class SupportRequestPage : Page
    {
        private SupportRequest selected_request;
        List<SupportRequestMessage> request_messages = new List<SupportRequestMessage>();
        List<SupportRequestMessage> request_messages_temp = new List<SupportRequestMessage>();
        NotificationManager notificationManager = new NotificationManager();
        ApiSupport api_request = new ApiSupport();
        Timer timer = new Timer();

        internal SupportRequestPage(SupportRequest selected_request)
        {
            InitializeComponent();
            this.selected_request = selected_request;
            LoadMessages();
            StackPanel_RequestInfo.DataContext = selected_request;

            if (ListView_Messages.Items.Count == 0)
            {
                notificationManager.Show("Сообщения не найдены", NotificationType.Information);
            }
        }

        private async void LoadMessages()
        {
            if (request_messages.Count != 0)
            {
                request_messages.Clear();
            }
            request_messages = await api_request.GetSupportMessages(selected_request.RequestId);

            ListView_Messages.ItemsSource = request_messages;
            ListView_Messages.ScrollIntoView(ListView_Messages.Items[ListView_Messages.Items.Count - 1]);
            timer.Interval = 5000; // Частота проверки в миллисекундах (здесь каждые 5 секунд)
            timer.Elapsed += Timer_TickAsync;
            timer.Start();

        }

        private async void Button_SendMessage_Click(object sender, RoutedEventArgs e)
        {
            /*            if (TextBox_Message.Text != null || TextBox_Message.Text == "")
                        {
                            return;
                        }*/
            if (TextBox_Message.Text == "" || TextBox_Message.Text == null)
            {
                notificationManager.Show("Сообщение не может быть пустым", NotificationType.Error);
                return;
            }
            SupportRequestMessage sended_message = await api_request.SendSupportMessage(selected_request.RequestId, message: TextBox_Message.Text);
            TextBox_Message.Text = "";
            if (sended_message.MessageContent == "")
            {
                return;
            }
            request_messages.Add(sended_message);
            ListView_Messages.Items.Refresh();

        }
        private async void Timer_TickAsync(object sender, EventArgs e)
        {
            // Вызываем метод для загрузки новых сообщений
            List<SupportRequestMessage> newMessages = await api_request.GetSupportMessages(selected_request.RequestId);
            LoadNewMessages(newMessages);
        }

        private void LoadNewMessages(List<SupportRequestMessage> newMessages)
        {
            // Загрузка новых сообщений
            if (newMessages.Count <= request_messages.Count)
            {
                return;
            }
            newMessages.RemoveRange(0, request_messages.Count);

            // Проверка наличия новых сообщений
            if (newMessages != null || newMessages.Count > 0)
            {
                // Добавление новых сообщений в список
                foreach (var message in newMessages)
                {
                    request_messages.Add(message);
                }

                Dispatcher.Invoke((Action)(() =>
                {
                    ListView_Messages.Items.Refresh();
                }));
            }
        }

        private void Button_Back_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new UserSupportServicePage());
        }
    }
}
