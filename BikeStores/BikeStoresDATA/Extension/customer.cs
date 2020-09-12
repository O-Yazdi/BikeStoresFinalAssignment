using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BikeStoresDATA.EF
{
    [MetadataType(typeof(customerMetaData))]
    public partial class customer
    {

    }

    public class customerMetaData
    {
        [RegularExpression(@"^[a - z\d -] +\@[a-z\d-]+(\.[a-z]{2,8})", ErrorMessage = "Invalid email")]
        public string email;
    }
}