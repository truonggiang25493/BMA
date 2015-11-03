using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BMA.Models.Metadata
{
    [MetadataTypeAttribute(typeof(ProductMetadata))]
    public partial class Product
    {
        internal sealed class ProductMetadata
        {
            public int ProductId { get; set; }
            [Required(ErrorMessage = "Tên sản phẩm không được để trống.")]
            [StringLength(100, ErrorMessage = "Tên sản phẩm có độ dài tối đa {1} ký tự")]
            public string ProductName { get; set; }
            [Required(ErrorMessage = "Trọng lượng của sản phẩm không được để trống")]
            [StringLength(4, MinimumLength = 3)]
            [DataType(DataType.Currency)]
            public double ProductWeight { get; set; }
            [Required(ErrorMessage = "Đơn vị tính của sản phẩm không được để trống")]
            [StringLength(50, MinimumLength = 5, ErrorMessage = "Đơn vị tính có độ dài 5 tới 50 ký tự")]
            public string Unit { get; set; }
            [StringLength(500)]
            public string Descriptions { get; set; }
            [StringLength(500)]
            public string Note { get; set; }
            public string ProductImgage { get; set; }
            [DataType(DataType.Currency)]
            [Required(ErrorMessage = "Giá của sản phẩm không được để trống")]
            public int ProductStandardPrice { get; set; }
            public Nullable<int> CategoryId { get; set; }
            public bool IsActive { get; set; }
            [Required(ErrorMessage = "Mã sản phẩm không được để trống")]
            [StringLength(50, ErrorMessage = "Mã sản phẩm có độ dài tối đa 50 ký tự")]
            public string ProductCode { get; set; }
        }
    }
}