using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Логика взаимодействия для AddUpdate.xaml
    /// </summary>
    public partial class AddUpdate : Page
    {
        public Client client;
        public List<Tag> tg = new List<Tag>();
        public AddUpdate(Client cl)
        {
            if (cl == null) client = new Client();
            else client = cl;
            this.DataContext = client;
            InitializeComponent();
            try
            {
                List<Gender> genders = new List<Gender> { };
                genders = helper.GetContext().Genders.ToList();
                for (int i = 0; i < 2; i++)
                {
                    Gender cln = genders[i];
                    ((RadioButton)Gender.Children[i]).Content = cln.Name;
                    ((RadioButton)Gender.Children[i]).Tag = cln.Code;
                    if (client.GenderCode == cln.Code) ((RadioButton)Gender.Children[i]).IsChecked = true;
                }
            }
            catch { };
            if (client.ID == 0)
            {
                btnDelAg.IsEnabled = false;
                btnWritHi.IsEnabled = false;
                btnDelHi.IsEnabled = false;
            }
            else
            {
                historyGrid.ItemsSource = client.Tags.ToList();
            }
            try
            {
                tg.Clear();
                tg = helper.GetContext().Tags.ToList();
            }
            catch { };

            foreach (Tag tag in tg)
            {
                TextBlock block = new TextBlock();
                block.Text = tag.Title;
                block.Tag = tag.ID;
                var converter = new System.Windows.Media.BrushConverter();
                SolidColorBrush hb = (SolidColorBrush)(Brush)converter.ConvertFromString("#FF" + tag.Color);
                block.Foreground = hb;
                product.Items.Add(block);
            }

        }
        private void btnWritAg_Click(object sender, RoutedEventArgs e)
        {
            if (client.FirstName.Length > 50 || client.LastName.Length > 50) return;
            if ((client.Patronymic != null) && (client.Patronymic != "") && client.Patronymic.Length > 50) return;
            if (client.FirstName.Length == 0 || client.LastName.Length == 0 || client.Phone.Length == 0) return;
            if ((client.ID == 0) && (client.GenderCode == null)) return;
            if (client.RegistrationDate == DateTime.MinValue || client.RegistrationDate.ToString().Length == 0) return;
            if ((client.Email != null) && (client.Email != "") && (!(new Regex(@"(\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*)")).IsMatch(client.Email))) return;
            if (!(new Regex(@"^\+?\d{0,2}\-?\d{3}\-?\d{3}\-?\d{4}")).IsMatch(client.Phone)) return;
            if ((new Regex(@"[^А-Яа-я- ]")).IsMatch(client.FirstName)) return;
            if ((new Regex(@"[^А-Яа-я- ]")).IsMatch(client.LastName)) return;
            if ((client.Patronymic != null) && ((new Regex(@"[^А-Яа-я- ]")).IsMatch(client.Patronymic))) return;
            try
            {
                if (client.ID > 0)
                {
                    helper.GetContext().Entry(client).State = EntityState.Modified;
                    helper.GetContext().SaveChanges();
                    MessageBox.Show("Обновление информации о клиенте завершено");
                }
                else
                {
                    helper.GetContext().Clients.Add(client);
                    helper.GetContext().SaveChanges();
                    MessageBox.Show("Добавление информации о клиенте завершено");
                }
            }
            catch { }
            btnDelAg.IsEnabled = true;
            btnWritHi.IsEnabled = true;
            btnDelHi.IsEnabled = true;
        }

        private void btnDelAg_Click(object sender, RoutedEventArgs e)
        {
            if (client.ClientServices.Count > 0)
            {
                MessageBox.Show("Информация о клиенте не может быть удалена");
                return;
            }
            helper.GetContext().Clients.Remove(client);
            helper.GetContext().SaveChanges();
            MessageBox.Show("Удаление информации о клиенте завешено!");
            this.NavigationService.GoBack();
        }


        private void product_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void btnWritHi_Click(object sender, RoutedEventArgs e)
        {
            if (product.SelectedItem != null)
            {
                string text = ((TextBlock)product.SelectedItem).Text;
                int id = Convert.ToInt32(((TextBlock)product.SelectedItem).Tag);
                Tag tag = tg.Find(item => item.ID == id);
                client.Tags.Add(tag);
                historyGrid.ItemsSource = client.Tags.ToList();
            }
        }


        private void btnDelHi_Click(object sender, RoutedEventArgs e)
        {
            if (historyGrid.SelectedItems.Count > 0)
            {
                Tag tag = historyGrid.SelectedItems[0] as Tag;
                client.Tags.Remove(tag);
                historyGrid.ItemsSource = client.Tags.ToList();
            }
        }


        private void historyGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void historyGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            Tag tag = (Tag)e.Row.DataContext;
            var converter = new System.Windows.Media.BrushConverter();
            SolidColorBrush hb = (SolidColorBrush)(Brush)converter.ConvertFromString("#FF" + tag.Color);
            e.Row.Foreground = hb;


        }


        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            client.GenderCode = ((RadioButton)sender).Tag.ToString();
        }

        private void PhotoBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                BitmapImage myBitmapImage1 = new BitmapImage();
                string path = openFileDialog.FileName;
                string filename = openFileDialog.SafeFileName;
                myBitmapImage1.BeginInit();
                myBitmapImage1.UriSource = new Uri(@openFileDialog.FileName, UriKind.Absolute);
                myBitmapImage1.EndInit();
                FileInfo fileInf = new FileInfo(@path);
                if (fileInf.Length > 2000000)
                {
                    MessageBox.Show("Размер файла больше 2 М");
                    return;
                }
                Photo.Source = myBitmapImage1;
                string message = "Сохранить изображение?";
                string caption = "Сохранение";
                MessageBoxButton buttons = MessageBoxButton.YesNo;

                if (MessageBox.Show(message, caption, buttons, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    FileInfo fileInfN = new FileInfo("../.");
                    string pathn = fileInfN.DirectoryName;
                    string tagpath = (pathn + "\\images\\" + filename);
                    try
                    {
                        fileInf.CopyTo(@tagpath, true);
                    }
                    catch { };
                    PhotoS.Source = Photo.Source;
                    Photo.Source = null;
                    client.PhotoPath = filename;
                }
                else
                {
                    Photo.Source = null;
                };
            }
        }


    }
}
