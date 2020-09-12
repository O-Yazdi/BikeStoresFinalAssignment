using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BikeStoresAPI.DTO
{
    public class InventoryMissingStoreDTO
    {
        public string store_name;
        public string product_name;
        public int product_id;
        public decimal list_price;
        public Nullable<int> store_quantity;
        public int order_quantity;
        public int amount_of_missing_units;
        public decimal sales_loss;
    }
}