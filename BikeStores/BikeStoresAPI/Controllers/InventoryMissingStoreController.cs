using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BikeStoresAPI.DTO;
using BikeStoresDATA.EF;

namespace BikeStoresAPI.Controllers
{
    public class InventoryMissingStoreController : ApiController
    {
        //POST api/<controller>
        public List<InventoryMissingStoreDTO> Post([FromBody] SerachProductStoreDTO searchValue)
        {
            string store_name_search = searchValue.store_name;
            string product_name_search = searchValue.product_name;

            BikeStoresDBContext db = new BikeStoresDBContext();

            var orderQuery = (from o in db.orders.Where(x => (x.order_status != 4) && (x.order_status != 3))
                              join oi in db.order_items on o.order_id equals oi.order_id
                              select new { order = o, order_id = oi }
                         ).ToList();

            var groups = orderQuery.GroupBy(x => new { product_id = x.order_id.product_id, store_id = x.order.store_id }).ToList();

            var productStock = (from s in db.stocks
                                join p in db.products on s.product_id equals p.product_id
                                select new { store_id = s.store_id, store_name = s.store.store_name, product_id = s.product_id, quantity = s.quantity, name = p.product_name, price = p.list_price }
                               ).ToList();

            var results = groups.SelectMany(x => productStock.Where(y => (x.Key.product_id == y.product_id) && (x.Key.store_id == y.store_id) && (x.Sum(z => z.order_id.quantity) > y.quantity))
                .Select(y => new InventoryMissingStoreDTO ()
                {
                    store_name = y.store_name,
                    product_name = y.name,
                    product_id = x.Key.product_id,
                    list_price = y.price,
                    store_quantity = y.quantity,
                    order_quantity = x.Sum(z => z.order_id.quantity),
                    amount_of_missing_units = x.Sum(z => z.order_id.quantity) - y.quantity ?? 0,
                    sales_loss = (x.Sum(z => z.order_id.quantity) - y.quantity) * y.price ?? 0,
                }).ToList()).ToList();

            // I used in IndexOf method instead of Contains method to make the filter case insensitive search
            var filterRes = results.Where(x => (x.store_name.IndexOf(store_name_search, StringComparison.OrdinalIgnoreCase) >= 0) && x.product_name.IndexOf(product_name_search, StringComparison.OrdinalIgnoreCase) >= 0).ToList();


            //It's not the most effective way to execute the query, but I wanted to keep it simple
            return filterRes;
        }
    }
}