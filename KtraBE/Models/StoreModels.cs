using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
namespace KtraBE.Models
{
    public class ShopHomeVM
    {
        public string Keyword { get; set; }
        public int? CategoryId { get; set; }
        public int Page { get; set; }
        public int PageCount { get; set; }
        public int TotalCount { get; set; }
        public List<Product> Products { get; set; } = new List<Product>();
        public List<Category> Categories { get; set; } = new List<Category>();
    }
    public class ShopProductVM
    {
        public Product Product { get; set; }
        public List<Product> Related { get; set; } = new List<Product>();
    }
    [Serializable]
    public class ShopCartItem
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public decimal ProductPrice { get; set; }
        public string ProductImage { get; set; }
        public int Quantity { get; set; }
        public decimal Total => ProductPrice * Quantity;
    }
    public class ShopCartVM
    {
        public List<ShopCartItem> Items { get; set; } = new List<ShopCartItem>();
        public int Count => Items.Sum(i => i.Quantity);
        public decimal Total => Items.Sum(i => i.Total);
    }
    public class CustomerRegisterVM
    {
        [Required(ErrorMessage="Nhập tên đăng nhập"), StringLength(50), Display(Name="Tên đăng nhập")]
        public string Username { get; set; }
        [Required(ErrorMessage="Nhập mật khẩu"), MinLength(6, ErrorMessage="Mật khẩu tối thiểu 6 ký tự"), DataType(DataType.Password), Display(Name="Mật khẩu")]
        public string Password { get; set; }
        [Required, Compare("Password",ErrorMessage="Xác nhận mật khẩu không khớp"), DataType(DataType.Password), Display(Name="Xác nhận mật khẩu")]
        public string ConfirmPassword { get; set; }
        [Required(ErrorMessage="Nhập họ tên"), Display(Name="Họ tên")]
        public string CustomerName { get; set; }
        [Required(ErrorMessage="Nhập số điện thoại"), StringLength(15), Display(Name="Số điện thoại")]
        public string CustomerPhone { get; set; }
        [EmailAddress(ErrorMessage="Email không hợp lệ"), Display(Name="Email")]
        public string CustomerEmail { get; set; }
        [Required(ErrorMessage="Nhập địa chỉ"), Display(Name="Địa chỉ")]
        public string CustomerAddress { get; set; }
    }
    public class CheckoutVM
    {
        [Required(ErrorMessage="Nhập địa chỉ nhận hàng"), Display(Name="Địa chỉ giao hàng")]
        public string AddressDelivery { get; set; }
        [Required] public string DeliveryMethod { get; set; } = "Giao tiêu chuẩn";
        [Required] public string PaymentMethod { get; set; } = "Tiền mặt";
        public ShopCartVM Cart { get; set; } = new ShopCartVM();
    }
}
