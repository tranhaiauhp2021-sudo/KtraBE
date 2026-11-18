using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace KtraBE.Models
{
    public class LoginVM
    {
        [Required(ErrorMessage = "Nhập tên đăng nhập")]
        [Display(Name = "Tên đăng nhập")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Nhập mật khẩu")]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu")]
        public string Password { get; set; }
    }

    public class OrderSearchVM
    {
        [Display(Name = "Tên khách hàng")]
        public string CustomerName { get; set; }

        [Display(Name = "Trạng thái thanh toán")]
        public string PaymentStatus { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Ngày đặt hàng")]
        public DateTime? OrderDate { get; set; }

        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int PageCount { get; set; }
        public List<Order> Orders { get; set; } = new List<Order>();
    }

    public class UpdateOrderStatusVM
    {
        public int OrderID { get; set; }
        public string CustomerName { get; set; }
        [Required(ErrorMessage = "Chọn trạng thái đơn hàng")]
        [Display(Name = "Trạng thái đơn hàng")]
        public string OrderStatus { get; set; }

        [Required(ErrorMessage = "Chọn trạng thái thanh toán")]
        [Display(Name = "Trạng thái thanh toán")]
        public string PaymentStatus { get; set; }
    }
}
