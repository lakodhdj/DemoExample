using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DemoExample.Msgs;
using DemoExample.Statics;
using DemoExample.ViewModels;
using System.Data.Entity;

namespace DemoExample
{
    /// <summary>
    /// Логика взаимодействия для ProductWindow.xaml
    /// </summary>
    public partial class ProductWindow : Window
    {

        // Создаем переменные для работы с базой, классом MessageHelper - простые короткие функции для ошибок и предупреждений и
        // ProductViewModels - класс для работы с таблицей Продуктов.
        private Entities _db = new Entities();
        private MessageHelper _mh = new MessageHelper();
        private List<ProductViewModels> _allProducts;

        // Список заранее прописанных вариантов сортировки
        private string[] _sortingTypes = new string[]
        {
            "По умолчанию",
            "По убыванию",
            "По возрастанию"
        };

        // Лист заранее прописанного варианта фильтрации, остальные буду подгружаться из БД
        private List<string> _filteringTypes = new List<string>()
        {
            "Все поставщики"
        };

        // Конструктор окна, создается автоматически, нужно только добавить LoadUI(), LoadProducts(), LoadData();
        public ProductWindow()
        {
            InitializeComponent();
            LoadUI();
            LoadProducts();
            LoadData();

        }

        // Загружаем Админ-панель только для менеджера и админа
        // LoadUI (Загрузка User Interface)
        private void LoadUI()
        {
            User user = CurrentSession.CurrentUser;
            if (user == null || user.RoleId == 3)
            {
                AdminPanel.Visibility = Visibility.Collapsed; // Если пользователь НЕ админ, то панель поиска, сортирвоки скрыта
                CreateButton.Visibility = Visibility.Collapsed; // Если пользователь НЕ админ, то кнопка Создать скрыта
            }
        }

        // Загрузку данных для сортировок и фильтрации
        // Сортировку подключаем автоматически, так как список есть заранее
        // Фильтры подключаем после загрузки данных из БД
        // Также для TextBox FullUserName присваиваем значение - НЕОБЯЗАТЕЛЬНО ДЕЛАТЬ

        private void LoadData()
        {
            SortingComboBox.ItemsSource = _sortingTypes;

            _filteringTypes.AddRange(_db.Provider.Select(p => p.name));
            FilteringComboBox.ItemsSource = _filteringTypes;

            var user = CurrentSession.CurrentUser;
            if (user != null)
            {
                FullUserName.Text = $"{user.LastName}  {user.FirstName}";
            }
        }

        // Загрузка продуктов из таблицы Product включаем напрямую связанные таблицы (Provider, Producer etc), чтобы не было null значений
        // Превращаем в список и помещаем в _allProducts, который является источником данных
        private void LoadProducts()
        {
            _allProducts = _db.Product
                .Include(p => p.Provider)
                .Include(p => p.Producer)
                .Include(p => p.Category)
                .Include(p => p.Unit)
                .ToList()
                .Select(p => new ProductViewModels(p))
                .ToList();

            ProductList.ItemsSource = _allProducts;
        }

        // При нажатии на кнопку "Выйти" текущий пользователь обнуляется
        // Открывается окно авторизации и закрывается окно продуктов
        private void LogOutButton_Click(object sender, RoutedEventArgs e)
        {
            CurrentSession.CurrentUser = null;
            new MainWindow().Show();
            Close();
        }

        // При нажатии на кнопку "Создать" открывается окно добавления/редактирования продукта, но с пустыми полями, так как id не передается
        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            new AddEditProductWindow(null).Show();
            Close();
        }

        // Функция, которая используется тремя элементами Админ-панели
        private void FilterChanged(object sender, EventArgs e) => ApplyFilters(); 


        private void ApplyFilters()
        {
            // Забираем продукты в переменную, которая подходит для запросов - AsQueryable()
            var query = _allProducts.AsQueryable();

            // Поиск
            // Забираем введенный текст в переменую, меняем регистр на Lower, чтобы избежать ошибок при поиске
            string text = SearchingTextBox.Text.ToLower();
            // Проверяем введеное значение на пустоту и делаем запрос
            if (!string.IsNullOrWhiteSpace(text))
            {
                // Ищем те продукты, которые совпадают по имени, описанию, поставщику, производителю
                query = query.Where(p =>
                    p.Name.ToLower().Contains(text) ||
                    p.Description.ToLower().Contains(text) ||
                    p.Provider.name.ToLower().Contains(text) ||
                    p.Producer.name.ToLower().Contains(text));
            }

            // Фильтр
            //Забиарем выбранный объект в переменную 
            var filter = (FilteringComboBox.SelectedItem as string).ToLower();

            // Проверяем, что не пусто и не равно ВСЕ ПОСТАВЩИКИ
            // Ищем все продукты, у которых такой же поставщик (Provider)
            if (!string.IsNullOrEmpty(filter) && filter != "все поставщики")
            {
                query = query.Where(p =>
                    (p.Provider.name).ToLower() == filter);
            }

            // Сортировка
            // Смотрим по выбранному по полю и сортируем по возрастанию или убыванию
            switch (SortingComboBox.SelectedIndex)
            {
                case 1:
                    query = query.OrderByDescending(p => p.StockQuantity);
                    break;
                case 2:
                    query = query.OrderBy(p => p.StockQuantity);
                    break;
            }

            // Отправляем данные в наш источник на Фронтенде
            ProductList.ItemsSource = query.ToList();
        }

        // Кнопка Редактирования
        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            // Проверка прав
            var user = CurrentSession.CurrentUser;
            if (user == null || user.id == 3)
            {
                MessageBox.Show("Нет прав для редактирования", "Доступ запрещён",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Безопасное получение id из Tag
            // При нажатии на кнопку автоматически передается АЙДИ продукта
            try
            {
                int id = (int)((Button)sender).Tag;
                new AddEditProductWindow(id).Show();
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка:\n" + ex.Message);
            }
        }

        // Кнопка Удалить
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            // Забираем по кнопке АЙДИ продукта
            int id = (int)((Button)sender).Tag;
            // Берем текущего юзера
            var user = CurrentSession.CurrentUser;

            // Проверяем пользователя
            if (user == null || user.RoleId == 3)
            {
                _mh.ShowWarning("Нет прав на удаление.");
                return;
            }

            // Окно подтверждения удаления 
            if (MessageBox.Show("Удалить товар?", "Подтверждение",
                MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
                return;

            // Удаление из БД и из отобржаемого списка продуктов
            try
            {
                var product = _db.Product.FirstOrDefault(p => p.id == id);
                if (product == null) return;

                _db.Product.Remove(product);
                _db.SaveChanges();

                var item = _allProducts.FirstOrDefault(p => p.id == id);
                if (item != null)
                {
                    _allProducts.Remove(item);
                    ApplyFilters();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка удаления:\n" + ex.Message);
            }
        }

        // Открытие панели Заказов
        private void OrderButton_Click(object sender, RoutedEventArgs e)
        {
            new OrderWindow().Show();
            Close();
        }
    }
}
