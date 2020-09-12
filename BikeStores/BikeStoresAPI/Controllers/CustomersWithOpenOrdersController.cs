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
    public class CustomersWithOpenOrdersController : ApiController
    {
        // POST api/<controller>
        public List<CustomersWithOpenOrdersDTO> Post([FromBody] SerachToUpdateTableDTO searchValue)
        {
            string store_name_search = searchValue.store_name;
            string customer_name_search = searchValue.customer_name;
            string customer_email_search = searchValue.customer_email;


            BikeStoresDBContext db = new BikeStoresDBContext();

            var subQuery = (from o in db.orders.Where(x => (x.order_status != 3) && (x.order_status != 4))
                              join oi in db.order_items on o.order_id equals oi.order_id
                              select new { order = o, order_items = oi }).ToList();

            var groups = subQuery.GroupBy(x => new
            { order_id = x.order_items.order_id, customer_id = x.order.customer_id,
                store_id = x.order.store_id, order_status = x.order.order_status, order_date = x.order.order_date }).ToList();



            var res = (from g in groups
                       join c in db.customers on g.Key.customer_id equals c.customer_id
                       join s in db.stores on g.Key.store_id equals s.store_id
                       select new CustomersWithOpenOrdersDTO()
                       {
                           customer_id = c.customer_id,
                           order_id = g.Key.order_id,
                           full_name = c.first_name + ' ' + c.last_name,
                           email = c.email,
                           store_name = s.store_name,
                           order_date = g.Key.order_date.ToShortDateString(),
                           total_price = g.Sum(z => z.order_items.list_price),
                           order_status = intStatusToString(g.Key.order_status),
                       }).ToList();


            var filterRes = res.Where(x => (x.store_name.IndexOf(store_name_search, StringComparison.OrdinalIgnoreCase) >= 0) &&
                     (x.full_name.IndexOf(customer_name_search, StringComparison.OrdinalIgnoreCase) >= 0 ||
                             x.full_name.IndexOf(customer_name_search, StringComparison.OrdinalIgnoreCase) >= 0)
                     && (x.email.IndexOf(customer_email_search, StringComparison.OrdinalIgnoreCase) >= 0)).ToList();

            return filterRes;
           }



        static string intStatusToString(int num)
        {
            switch (num)
            {
                case 1: return "pending";
                case 2: return "processing";
                case 3: return "canceled";
                case 4: return "completed";
                default: return "EROR";
            }
        }
    }
}

