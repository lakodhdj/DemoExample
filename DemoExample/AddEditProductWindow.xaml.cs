using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
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
using DemoExample;

namespace DemoExample
{
    /// <summary>
    /// Логика взаимодействия для AddEditProductWindow.xaml
    /// </summary>
    public partial class AddEditProductWindow : Window
    {
        private Entities _db = new Entities();
        private bool _isEditing;
        private Product _product;
        public AddEditProductWindow(int? id)
        {
            InitializeComponent();

            if (id == null)
            {
                _isEditing = false;
            }
            else
            {
                _isEditing = true;
                _product = _db.Product.Find(id);
            }

            LoadData();
        }

        private void LoadData()
        {
            var units = _db.Unit.ToList();
            var categories = _db.Category.ToList();
            var producers = _db.Producer.ToList();
            var providers = _db.Provider.ToList();


            ProductUnit.ItemsSource = units;
            ProductUnit.SelectedIndex = 0;
            ProductUnit.DisplayMemberPath = "name";
            ProductUnit.SelectedValuePath = "id";

            ProductProducer.ItemsSource = producers;
            ProductProducer.SelectedIndex = 0;
            ProductProducer.DisplayMemberPath = "name";
            ProductProducer.SelectedValuePath = "id";

            ProductProvider.ItemsSource = providers;
            ProductProvider.SelectedIndex = 0;
            ProductProvider.DisplayMemberPath = "name";
            ProductProvider.SelectedValuePath = "id";

            ProductCategory.ItemsSource = categories;
            ProductCategory.SelectedIndex = 0;
            ProductCategory.DisplayMemberPath = "name";
            ProductCategory.SelectedValuePath = "id";

            if (_isEditing == true)
                FillData();
        }

        private void FillData()
        {
            ProductArticle.Text = _product.article;
            ProductName.Text = _product.Name;
            ProductPrice.Text = _product.Price.ToString();
            ProductDiscount.Text = _product.DIscount.ToString();
            ProductStockQuantuty.Text = _product.StockQuantity.ToString();
            ProductDesc.Text = _product.Description.ToString();
            ProductPhoto.Text = _product.Photo;

            ProductUnit.SelectedValue = _product.UnitId;
            ProductProducer.SelectedValue = _product.ProducerId;
            ProductProvider.SelectedValue = _product.ProviderId;
            ProductCategory.SelectedValue = _product.CategoryId;

        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            new ProductWindow().Show();
            Close();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {

            if (_isEditing == true)
            {
                UpdateProduct();
            }
            else
                CreateProduct();
        }

        private void CreateProduct()
        {
            Product product = new Product();

            string article = ProductArticle.Text;
            string name = ProductName.Text;
            int price = Convert.ToInt32(ProductPrice.Text);
            int discount = Convert.ToInt32(ProductDiscount.Text);
            int quantity = Convert.ToInt32(ProductStockQuantuty.Text);
            string desc = ProductDesc.Text;

            product.article = article;
            product.Price = price;
            product.Name = name;
            product.DIscount = discount;
            product.Description = desc;
            product.StockQuantity = quantity;

            product.UnitId = (int)ProductUnit.SelectedValue;
            product.ProducerId = (int)ProductProducer.SelectedValue;
            product.ProviderId = (int)ProductProvider.SelectedValue;
            product.CategoryId = (int)ProductCategory.SelectedValue;

            _db.Product.AddOrUpdate(product);
            _db.SaveChanges();
            CancelButton_Click(null, null);

        }

        private void UpdateProduct()
        {
            string article = ProductArticle.Text;
            string name = ProductName.Text;
            int price = Convert.ToInt32(ProductPrice.Text);
            int discount = Convert.ToInt32(ProductDiscount.Text);
            int quantity = Convert.ToInt32(ProductStockQuantuty.Text);
            string desc = ProductDesc.Text;

            _product.article = article;
            _product.Price = price;
            _product.Name = name;
            _product.DIscount = discount;
            _product.Description = desc;
            _product.StockQuantity = quantity;

            _product.UnitId = (int)ProductUnit.SelectedValue;
            _product.ProducerId = (int)ProductProducer.SelectedValue;
            _product.ProviderId = (int)ProductProvider.SelectedValue;
            _product.CategoryId = (int)ProductCategory.SelectedValue;

            _db.Product.AddOrUpdate(_product);
            _db.SaveChanges();
            CancelButton_Click(null, null);
        }

    }
}