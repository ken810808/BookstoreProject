//------------------------------------------------------------------------------
// <auto-generated>
//     這個程式碼是由範本產生。
//
//     對這個檔案進行手動變更可能導致您的應用程式產生未預期的行為。
//     如果重新產生程式碼，將會覆寫對這個檔案的手動變更。
// </auto-generated>
//------------------------------------------------------------------------------

namespace WebApplication1.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Absence_Type
    {
        public Absence_Type()
        {
            this.Absence_Table = new HashSet<Absence_Table>();
            this.CheckInDetail = new HashSet<CheckInDetail>();
        }
    
        public int Absence_ID { get; set; }
        public string Absence_Type1 { get; set; }
    
        public virtual ICollection<Absence_Table> Absence_Table { get; set; }
        public virtual ICollection<CheckInDetail> CheckInDetail { get; set; }
    }
}
