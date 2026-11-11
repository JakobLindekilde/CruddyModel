using System.ComponentModel;

namespace CruddyModelDemo.Models;

public partial class Product
{
    public int Id { get; set; }

    [DisplayName("Product")]
    public string Name { get; set; } = null!;

    public int Price { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
