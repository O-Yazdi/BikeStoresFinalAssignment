using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BikeStoresAPI.DTO;
using BikeStoresDATA.EF;

namespace BikeStoresAPI.Controllers
{
    public class UpdateOrdersController : ApiController
    {

        public HttpResponseMessage Post([FromBody] UpdateDetailsDTO newValues)
        {
            BikeStoresDBContext db = new BikeStoresDBContext();

            customer c = db.customers.Where(x => x.customer_id == newValues.customer_id).SingleOrDefault();
            if (c == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "לא קיים לקוח עם הפרטים שנשלחו");
            }

            order o = db.orders.Where(y => y.order_id == newValues.order_id).SingleOrDefault();
            if (c == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "לא קיימת הזמנה עם הפרטים שנשלחו");
            }

            if (o.order_status == 3 || o.order_status == 4)
            {
                return Request.CreateResponse(HttpStatusCode.Conflict, "סטטוס הזמנה לא מאפשר שינויים");
            }


            if (newValues.order_status == "completed")
            {
                var productsOrderStock = (from order in db.orders.Where(x => x.order_id == newValues.order_id)
                                          join order_item in db.order_items on order.order_id equals order_item.order_id
                                          join s in db.stocks on new { order.store_id, order_item.product_id } equals new { s.store_id, s.product_id }
                                          select new
                                          {
                                              storeID = order.store_id,
                                              orderID = order.order_id,
                                              customerID = order.customer_id,
                                              productID = order_item.product_id,
                                              quantityInOrder = order_item.quantity,
                                              UnitsInStock = s.quantity
                                          }).ToList();

                foreach (var x in productsOrderStock)
                {
                    if(x.quantityInOrder > x.UnitsInStock)
                    {
                        //According to a search I conducted, we should return a response that the request is correct (200),
                        //with a message that the product is out of stock
                        return Request.CreateResponse(HttpStatusCode.OK, "לא ניתן להשלים את ההזמנה, חסרים פריטים בחנות");
                    }
                }
                foreach(var x in productsOrderStock)
                {
                    stock s = db.stocks.Where(y => y.store_id == x.storeID && y.product_id == x.productID).SingleOrDefault();

                    s.quantity = s.quantity - x.quantityInOrder;
                }
                o.order_status = 4;
            }
            else
            {
                switch (newValues.order_status)
                {
                    case "pending":
                        o.order_status = 1; break;
                    case "processing":
                        o.order_status = 2; break;
                    case "canceled":
                        o.order_status = 3; break;
                    default:
                        return Request.CreateResponse(HttpStatusCode.BadRequest, "סטטוס הזמנה לא קיים");
                }
            }
            c.email = newValues.email;

            try
            {
                //try catch on the save changes
                db.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {
                string errors = "";
                foreach (DbEntityValidationResult vr in ex.EntityValidationErrors)
                {
                    foreach (DbValidationError er in vr.ValidationErrors)
                    {
                        errors += $"PropertyName - {er.PropertyName }, Error {er.ErrorMessage} <br/>";
                    }
                }
                return Request.CreateResponse(HttpStatusCode.BadRequest, errors);
            }
            catch(DbUpdateConcurrencyException ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "הטבלה שניסית לעדכן, לא מעודכנת, שינויים לא נשמרו");
            }
            catch (DbUpdateException ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.InnerException.Message);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.Conflict, "שגיאה בשמירת הנתונים");
            }

            //finish
            return Request.CreateResponse(HttpStatusCode.OK, "הנתונים נשמרו בהצלחה");
        }

    }
}