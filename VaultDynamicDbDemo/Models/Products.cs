using System.ComponentModel.DataAnnotations.Schema;

namespace VaultDynamicDbDemo.Models
{
    [Table("products", Schema = "public")]
    public class Products
    {
        public int id { get; set; }
        public string name { get; set; }
    }
}
