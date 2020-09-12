using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BikeStoresAPI.DTO
{
    public class CustomersWithOpenOrdersDTO
    {
        public int customer_id { get; set; }
        public int order_id { get; set; }
        public string full_name { get; set; }
        public string email { get; set; }
        public string store_name { get; set; }
        public string order_date { get; set; }
        public string order_status { get; set; }
        public decimal total_price { get; set; }
    }
}