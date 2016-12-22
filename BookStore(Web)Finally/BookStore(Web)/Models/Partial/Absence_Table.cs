using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BookStore_Web_.Models
{
    [MetadataType(typeof(MetaAbsence))]
    public partial class Absence_Table
    {
        public class MetaAbsence
        {
            [DisplayName("請假單號")]
            public int Absence_No { get; set; }

            [DisplayName("員工編號")]
            public int EmployeeID { get; set; }

            [Required(ErrorMessage = "請選擇請假類別")]
            [DisplayName("請假類別")]
            public int Absence_ID { get; set; }

            [Required(ErrorMessage = "請選擇請假起始日")]
            [DisplayName("請假起始日期、時間")]
            public System.DateTime StartDate { get; set; }

            [Required(ErrorMessage = "請選擇請假結束日")]
            [DisplayName("請假結束日期、時間")]
            public System.DateTime EndDate { get; set; }

            [StringLength(300, ErrorMessage = "字數不得超過300")]
            [DisplayName("請假原因")]
            [DataType(DataType.MultilineText)]
            [Required(ErrorMessage = "請填寫請假原因")]
            public string Reason { get; set; }

            [DisplayName("請假證明")]
            public byte[] Certificate_Doc { get; set; }

            [DisplayName("審核狀態")]
            public Nullable<int> Check_ID { get; set; }

            [DisplayName("請假類別")]
            public virtual Absence_Type Absence_Type { get; set; }
        }
    }
}