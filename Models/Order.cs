using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrderManagementAPI.Models
{
    public class Order
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [ForeignKey(nameof(Customer))]
        public int CustomerId { get; set; }

        [Required]
        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        // Both timestamps now default to UtcNow at creation, consistent
        // with every other entity in this project (Customer, Product).
        // The original had no default at all, meaning both would be
        // DateTime.MinValue (0001-01-01) unless explicitly set elsewhere -
        // an easy-to-miss bug.
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // "virtual" removed - it only has an effect if you've explicitly
        // enabled EF Core's lazy-loading proxies (UseLazyLoadingProxies()),
        // which this project doesn't. Left in place, it's misleading -
        // it looks like lazy loading is active when it isn't.
        public Customer Customer { get; set; } = null!;

        // Missing entirely in the original - an Order with no way to
        // reference its line items isn't a usable design. This is the
        // "many" side of Order's one-to-many relationship with OrderItem.
        public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();

        // Optimistic concurrency token - same pattern already used on
        // Product.RowVersion. Prevents two simultaneous status updates
        // (e.g., two staff members both processing the same order) from
        // silently overwriting each other.
        [Timestamp]
        public uint RowVersion { get; set; }
    }
}
