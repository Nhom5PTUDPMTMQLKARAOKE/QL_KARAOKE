//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WindowsFormsApp1
{
    using System;
    using System.Collections.Generic;
    
    public partial class ThietBi_Phong
    {
        public string MaPhong { get; set; }
        public string MaThietBi { get; set; }
        public Nullable<int> SoLuong { get; set; }
    
        public virtual Phong Phong { get; set; }
        public virtual ThietBi ThietBi { get; set; }
    }
}
