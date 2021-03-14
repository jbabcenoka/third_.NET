using System;
using System.Collections.Generic;

#nullable disable

namespace TresaisMajasDarbs.Models
{
    
    public partial class Order
    {

        public Order()
        {
            OrderDetails = new HashSet<OrderDetail>();
        }

        public int Id { get; set; }
        public string Number { get; set; }
        public int State { get; set; }
        public DateTime OrderDate { get; set; }
        public int? CustomerId { get; set; }
        public int? ResponsibleEmployeeId { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual Employee ResponsibleEmployee { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
