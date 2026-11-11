using System.ComponentModel;

namespace CruddyModelDemo.Models;

public partial class Employee
{
    public int Id { get; set; }

    [DisplayName("Employee")]
    public string Name { get; set; } = null!;

    public Decimal Salary { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
