using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using BookStore_Web_.Models;

namespace BookStore_Web_.Models
{
    [MetadataType(typeof(MetaEmployees))]
    public partial class Emp_Information
    {
        public class MetaEmployees
        {
            [Required(ErrorMessage="請輸入編號")]
            [DisplayName("員工編號")]
            public int EmployeeID { get; set; }
            [DisplayName("員工職位")]
            [Required(ErrorMessage="一定要選擇")]
            public int Emp_OccupationID { get; set; }
            [DisplayName("員工姓名")]
            [Required(ErrorMessage = "請輸入姓名")]
            public string Emp_Name { get; set; }
            [DisplayName("家裡電話")]
            [Required(ErrorMessage = "必要欄位")]
            public string Emp_Tel { get; set; }
            [DisplayName("手機號碼")]
            [Required(ErrorMessage = "必要欄位")]
            public string Emp_CellPhone { get; set; }
            [DisplayName("地址")]
            [Required(ErrorMessage = "必要欄位")]
            public string Emp_Address { get; set; }
            [DisplayName("E-mail")]
            [Required(ErrorMessage = "必要欄位")]
            public string Emp_E_Mail { get; set; }
            [DisplayName("性別")]
            [Required(ErrorMessage = "必要欄位")]
            public bool Emp_Gender { get; set; }
            [DisplayName("開始工作日期")]
            [Required(ErrorMessage = "請選擇日期")]
            [DataType(DataType.Date)]
            public System.DateTime Emp_StartWorkingDate { get; set; }
            [DisplayName("大頭照")]
            public byte[] Emp_Image { get; set; }
            [DisplayName("部門")]
            [Required(ErrorMessage = "請選擇部門")]
            public Nullable<int> Emp_DeptID { get; set; }
            public Nullable<int> Emp_Report { get; set; }
            [DisplayName("員工密碼")]
            [Required(ErrorMessage = "請輸入密碼")]
            public string Emp_Password { get; set; }
            [DisplayName("身分證")]
            [Required(ErrorMessage = "必要欄位")]
            public string Emp_Identity { get; set; }
            [DisplayName("員工帳號")]
            [Required(ErrorMessage = "必要欄位")]
            public string Emp_Log { get; set; }
            public bool Emp_Dispose { get; set; }
        }
       
    }
}