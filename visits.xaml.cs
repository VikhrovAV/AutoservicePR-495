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
    /// Логика взаимодействия для visits.xaml
    /// </summary>
    public partial class visits : Page
    {
        public visits()
        {
            InitializeComponent();
        }

        private void visitGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            ClientService clSrv = (ClientService)e.Row.DataContext;
            clSrv.files = clSrv.DocumentByServices.Count();
        }

        private void visitGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (visitGrid.SelectedItems.Count > 0)
            {
                ClientService clSrv = visitGrid.SelectedItems[0] as ClientService;
                fileGrid.ItemsSource = clSrv.DocumentByServices.ToList();
            }
        }

        private void fileGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {

        }
    }
}
