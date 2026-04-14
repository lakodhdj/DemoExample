using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoExample.ViewModels
{
    public class OrderViewModels
    {
        public OrderViewModels(Order order)
        {
            id = order.id;
            OrderDate = order.OrderDate;
            OrderDelivery = order.OrderDelivery;
            PickUpAdressID = order.PickUpAdressID;
            CustomerID = order.CustomerID;
            Code = order.Code;
            StatusID = order.StatusID;

            this.OrderStatus = order.OrderStatus;
            this.PickUpPoint = order.PickUpPoint;
        }

        public int id { get; set; }
        public Nullable<System.DateTime> OrderDate { get; set; }
        public Nullable<System.DateTime> OrderDelivery { get; set; }
        public Nullable<int> PickUpAdressID { get; set; }
        public Nullable<int> CustomerID { get; set; }
        public Nullable<int> Code { get; set; }
        public Nullable<int> StatusID { get; set; }

        public virtual OrderStatus OrderStatus { get; set; }
        public virtual PickUpPoint PickUpPoint { get; set; }
    }
}
