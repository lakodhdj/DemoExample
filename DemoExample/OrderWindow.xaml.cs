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
using System.Windows.Shapes;
using DemoExample.Statics;
using DemoExample.ViewModels;
using System.Data.Entity;

namespace DemoExample
{
    /// <summary>
    /// Логика взаимодействия для OrderWindow.xaml
    /// </summary>
    /// 

    // ТАКОЙ ЖЕ ФАЙЛ, КАК И ProductWindow
    public partial class OrderWindow : Window
    {
        private Entities _db  = new Entities();
        private List<OrderViewModels> _allOrders;

        private string[] _sortingTypes = new string[]
        {
            "По умолчанию",
            "По убыванию",
            "По возрастанию"
        };

        private List<string> _filteringTypes = new List<string>()
        {
            "Все пункты выдачи"
        };

        public OrderWindow()
        {
            InitializeComponent();
            LoadUI();
            LoadOrders();
            LoadData();
        }

        private void FilterChanged(object sender, EventArgs e) => ApplyFilters();

        private void LoadUI()
        {
            User user = CurrentSession.CurrentUser;
            if (user == null || user.RoleId == 3)
            {
                AdminPanelOrder.Visibility = Visibility.Collapsed;


            }
            else
            {
                CreateButton.Visibility = Visibility.Visible;
            }
        }

        private void LoadData()
        {
            SortingComboBox.ItemsSource = _sortingTypes;

            _filteringTypes.AddRange(_db.PickUpPoint.Select(p => p.City));
            FilteringComboBox.ItemsSource = _filteringTypes;

            var user = CurrentSession.CurrentUser;
            if (user != null)
            {
                FullUserName.Text = $"{user.FirstName} {user.LastName}";
            }
        }
        private void LoadOrders()
        {
            _allOrders = _db.Order
                .Include(o => o.OrderStatus)
                .Include(o => o.PickUpPoint)
                .ToList()
                .Select(o => new OrderViewModels(o))
                .ToList();

            OrderList.ItemsSource = _allOrders;
        }

        private void ApplyFilters()
        {
            var query = _allOrders.AsQueryable();

            // Searching
            string text = SearchingTextBox.Text.ToLower();
            query = query.Where(o =>
                o.Code.ToString().ToLower().Contains(text) ||
                o.PickUpPoint.City.ToLower().Contains(text) ||
                o.PickUpPoint.Street.ToLower().Contains(text) ||
                o.PickUpPoint.House.ToString().ToLower().Contains(text) ||
                o.OrderStatus.name.ToLower().Contains(text));

            // Sorting
            switch (SortingComboBox.SelectedIndex)
            {
                case 1:
                    query = query.OrderByDescending(o => o.OrderDate);
                    break;
                case 2:
                    query = query.OrderBy(o => o.OrderDate);
                    break;
            }

            // Filtering
            var filter = (FilteringComboBox.SelectedValue as string)?.Trim().ToLower();
            if (!string.IsNullOrWhiteSpace(filter) && filter != "Все пункты выдачи")
            {
                query = query.Where(o => (o.PickUpPoint.City ?? "").Trim().ToLower() == filter);
            }

            OrderList.ItemsSource = query.ToList();

        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            new ProductWindow().Show();
            Close();
        }

        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
