using System;
using System.Collections.Generic;

#nullable disable

namespace TresaisMajasDarbs.Models
{
    public partial class Employee
    {
        public Employee()
        {
            Orders = new HashSet<Order>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public DateTime AgreementDate { get; set; }
        public string AgreementNr { get; set; }

        public virtual ICollection<Order> Orders { get; set; }
    }
}
