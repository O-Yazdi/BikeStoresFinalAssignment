using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BikeStoresAPI.DTO
{
    public class UpdateDetailsDTO
    {
        public int customer_id { get; set; }
        public int order_id { get; set; }
        public string email { get; set; }
        public string order_status { get; set; }
    }
}