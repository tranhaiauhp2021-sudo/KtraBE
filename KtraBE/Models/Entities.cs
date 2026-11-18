using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;

namespace KtraBE.Models
{
    // EF6 Code First ánh xạ vào database SQL có sẵn (không tự tạo database).
    [Table("User")]
    public class UserAccount
    {
        [Key, StringLength(255)] public string Username { get; set; }
        [Required, StringLength(255)] public string Password { get; set; }
        [Required, StringLength(1)] public string UserRole { get; set; }
    }

    [Table("Customer")]
    public class Customer
    {
        [Key] public int CustomerID { get; set; }
        [Required] public string CustomerName { get; set; }
        [Required, StringLength(15)] public string CustomerPhone { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerAddress { get; set; }
        [Required, StringLength(255)] public string Username { get; set; }
        [ForeignKey("Username")] public virtual UserAccount Account { get; set; }
        public virtual ICollection<Order> Orders { get; set; } = new HashSet<Order>();
    }

    [Table("Category")]
    public class Category
    {
        [Key] public int CategoryID { get; set; }
        [Required] public string CategoryName { get; set; }
    }

    [Table("Product")]
    public class Product
    {
        [Key] public int ProductID { get; set; }
        [Required] public int CategoryID { get; set; }
        [Required] public string ProductName { get; set; }
        // Giữ nguyên cách viết trong database gốc: ProductDecription (thiếu chữ s).
        [Required] public string ProductDecription { get; set; }
        public decimal ProductPrice { get; set; }
        public string ProductImage { get; set; }
        [ForeignKey("CategoryID")] public virtual Category Category { get; set; }
    }

    [Table("Order")]
    public class Order
    {
        [Key] public int OrderID { get; set; }
        public int CustomerID { get; set; }
        [Column(TypeName = "date")] public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string PaymentStatus { get; set; }
        [Required] public string AddressDelivery { get; set; }
        [StringLength(40)] public string OrderStatus { get; set; }
        [StringLength(60)] public string PaymentMethod { get; set; }
        [StringLength(60)] public string DeliveryMethod { get; set; }
        [ForeignKey("CustomerID")] public virtual Customer Customer { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new HashSet<OrderDetail>();
    }

    [Table("OrderDetail")]
    public class OrderDetail
    {
        [Key] public int ID { get; set; }
        public int ProductID { get; set; }
        public int OrderID { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        // Cột tính tự động từ SQL (Quantity * UnitPrice).
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)] public decimal TotalPrice { get; set; }
        [ForeignKey("OrderID")] public virtual Order Order { get; set; }
        [ForeignKey("ProductID")] public virtual Product Product { get; set; }
    }

    public class MyStoreContext : DbContext
    {
        public MyStoreContext() : base("name=MyStoreDb") { }
        public DbSet<UserAccount> Users { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>().Property(x => x.ProductPrice).HasPrecision(18, 2);
            modelBuilder.Entity<Order>().Property(x => x.TotalAmount).HasPrecision(18, 2);
            modelBuilder.Entity<OrderDetail>().Property(x => x.UnitPrice).HasPrecision(18, 2);
            modelBuilder.Entity<OrderDetail>().Property(x => x.TotalPrice).HasPrecision(29, 2);
            modelBuilder.Entity<Customer>().HasRequired(x => x.Account).WithMany().HasForeignKey(x => x.Username).WillCascadeOnDelete(false);
            modelBuilder.Entity<Order>().HasRequired(x => x.Customer).WithMany(x => x.Orders).HasForeignKey(x => x.CustomerID).WillCascadeOnDelete(false);
            modelBuilder.Entity<OrderDetail>().HasRequired(x => x.Order).WithMany(x => x.OrderDetails).HasForeignKey(x => x.OrderID).WillCascadeOnDelete(false);
            modelBuilder.Entity<OrderDetail>().HasRequired(x => x.Product).WithMany().HasForeignKey(x => x.ProductID).WillCascadeOnDelete(false);
            modelBuilder.Entity<Product>().HasRequired(x => x.Category).WithMany().HasForeignKey(x => x.CategoryID).WillCascadeOnDelete(false);
            base.OnModelCreating(modelBuilder);
        }
    }
}
