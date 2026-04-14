using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Xml.Linq;

namespace DemoExample.ViewModels
{
    public class ProductViewModels
    {
        public ProductViewModels(Product product)
        {
            id = product.id;
            article = product.article;
            Name = product.Name;
            UnitId = product.UnitId;
            Price = product.Price;
            ProviderId = product.ProviderId;
            ProducerId = product.ProducerId;
            CategoryId = product.CategoryId;
            DIscount = product.DIscount;
            StockQuantity = product.StockQuantity;
            Description = product.Description;
            Photo = product.Photo;

            this.Producer = product.Producer;
            this.Provider = product.Provider;
            this.Category = product.Category;
            this.Unit = product.Unit;


            GetBackground();
            GetPhoto();




        }

        public int id { get; set; }
        public string article { get; set; }
        public string Name { get; set; }
        public Nullable<int> UnitId { get; set; }
        public Nullable<int> Price { get; set; }
        public Nullable<int> ProviderId { get; set; }
        public Nullable<int> ProducerId { get; set; }
        public Nullable<int> CategoryId { get; set; }
        public Nullable<int> DIscount { get; set; }
        public Nullable<int> StockQuantity { get; set; }
        public string Description { get; set; }
        public string Photo { get; set; }

        public Brush Background { get; set; }

        public virtual Category Category { get; set; }
        public virtual Producer Producer { get; set; }
        public virtual Provider Provider { get; set; }
        public virtual Unit Unit { get; set; }
        public virtual ICollection<ProductInOrder> ProductInOrder { get; set; }

        private void GetBackground()
        {
            if (DIscount >= 15)
            {
                Background = (Brush)new BrushConverter().ConvertFrom("#2E8B57");
                return;
            }
            else if (StockQuantity == 0)
            {
                Background = Brushes.LightBlue; 
                return;
            }
        }

        private void GetPhoto()
        {
            if (!string.IsNullOrEmpty(Photo))
                return;
            Photo = "/Res/picture.png";
        }

    }



}
