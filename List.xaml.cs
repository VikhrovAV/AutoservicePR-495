using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Autoservice
{
    /// <summary>
    /// Логика взаимодействия для List.xaml
    /// </summary>
    public partial class List : Page
    {
        private string fnd = "";
        private int order = 0;
        private int start = 0;
        private int fullCount = 0;
        private string gender = "0";
        private int fullCountD = 0;
        private bool birthday = false;
        private int step = 10;
        private Frame frame;
        public List(Frame frame)
        {
            this.frame = frame;

            InitializeComponent();
            List<Gender> genders = new List<Gender> { };
            try
            {
                genders = helper.GetContext().Genders.ToList();
                genders.Add(new Gender { Name = "Все", Code = "0" });
                Gender.ItemsSource = genders.OrderBy(Gender => Gender.Code);
            }
            catch
            {

            }
            Load();
        }
        public void Load()
        {
            List<Client> clients = new List<Client>();
            try
            {
                fullCountD = helper.GetContext().Clients.Count();

                var ag = helper.GetContext().Clients.Where(c => (c.FirstName.Contains(fnd)) || (c.LastName.Contains(fnd)) || (c.Patronymic.Contains(fnd)) || (c.Phone.Contains(fnd)) || (c.Email.Contains(fnd)));
                if (birthday) ag = ag.Where(c => c.Birthday.Value.Month == DateTime.Now.Month);
                if (gender != "0") ag = ag.Where(c => c.GenderCode == gender);
                fullCount = ag.Count();
                clients.Clear();
                foreach (Client client in ag)
                {
                    client.ClServCn = client.ClientServices.Count();
                    client.ClServDt = DateTime.MinValue;
                    foreach (ClientService clientService in client.ClientServices)
                    {
                        if (clientService.StartTime > client.ClServDt) client.ClServDt = clientService.StartTime;
                    }
                    clients.Add(client);
                };
                if (order == 0) clientGrid.ItemsSource = ag.OrderBy(Client => Client.ID).Skip(start * step).Take(step).ToList();
                if (order == 1) clientGrid.ItemsSource = ag.OrderBy(Client => Client.FirstName).Skip(start * step).Take(step).ToList();
                if (order == 2 || order == 3)
                {
                    clients.Sort(comp);
                    clientGrid.ItemsSource = clients.Skip(start * step).Take(step).ToList();
                }
            }
            catch { };

            full.Text = fullCountD.ToString();
            fullr.Text = fullCount.ToString();
            int ost = fullCount % step;
            int pag = (fullCount - ost) / step;
            if (ost > 0) pag++;
            pagin.Children.Clear();
            for (int i = 0; i < pag; i++)
            {
                Button myButton = new Button();
                myButton.Height = 30;
                myButton.Content = i + 1;
                myButton.Width = 20;
                myButton.HorizontalAlignment = HorizontalAlignment.Center;
                myButton.Tag = i;
                myButton.Click += new RoutedEventHandler(paginButton_Click); ;
                pagin.Children.Add(myButton);
            }
            turnButton();

        }

        private int comp(Client x, Client y)
        {
            //            throw new NotImplementedException();
            if (order == 3)
            {
                if (x.ClServCn == y.ClServCn) return 0;
                if (x.ClServCn > y.ClServCn) return -1;
                return 1;
            }
            if (x.ClServDt == y.ClServDt) return 0;
            if (x.ClServDt > y.ClServDt) return -1;
            return 1;
        }

        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            frame.Content = new AddUpdate(null);
        }

        private void back_Click(object sender, RoutedEventArgs e)
        {
            start--;
            Load();
        }

        private void forward_Click(object sender, RoutedEventArgs e)
        {
            start++;
            Load();
        }
        private void paginButton_Click(object sender, RoutedEventArgs e)
        {
            start = Convert.ToInt32(((Button)sender).Tag.ToString());
            Load();
        }
        private void turnButton()
        {
            if (start == 0) { back.IsEnabled = false; }
            else { back.IsEnabled = true; };
            if ((start + 1) * 10 > fullCount) { forward.IsEnabled = false; }
            else { forward.IsEnabled = true; };
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            fnd = ((TextBox)sender).Text;
            start = 0;
            Load();
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            ComboBoxItem selectedItem = (ComboBoxItem)comboBox.SelectedItem;
            order = Convert.ToInt32(selectedItem.Tag.ToString());
            start = 0;
            Load();
        }

        private void BirthDay_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            ComboBoxItem selectedItem = (ComboBoxItem)comboBox.SelectedItem;
            int i = Convert.ToInt32(selectedItem.Tag.ToString());
            if (i == 0) { birthday = false; }
            else { birthday = true; };
            start = 0;
            Load();
        }
        private void Gender_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            gender = ((Gender)comboBox.SelectedItem).Code;
            start = 0;
            Load();
        }

        private void clientGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            Client client = (Client)e.Row.DataContext;
            TextBlock panel = new TextBlock();
            panel.TextWrapping = TextWrapping.Wrap;
            var converter = new System.Windows.Media.BrushConverter();
            foreach (Tag tag in client.Tags)
            {
                Run run = new Run();
                run.Text = tag.Title + " ";
                try
                {
                    run.Foreground = (Brush)converter.ConvertFromString("#FF" + tag.Color);
                }
                catch { };
                panel.Inlines.Add(run);
            };
            client.tags = panel;
        }

        private void Step_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            ComboBoxItem selectedItem = (ComboBoxItem)comboBox.SelectedItem;
            step = Convert.ToInt32(selectedItem.Tag.ToString());
            start = 0;
            Load();
        }

        private void updateButton_Click(object sender, RoutedEventArgs e)
        {
            if (clientGrid.SelectedItems.Count > 0)
            {
                Client client = clientGrid.SelectedItems[0] as Client;

                if (client != null)
                {
                    frame.Content = new AddUpdate(client);
                }
            }
        }
    }

}
