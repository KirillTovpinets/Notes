//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ForteGroupTest.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Notifications
    {
        public int NotifId { get; set; }
        public int RecieverId { get; set; }
        public int RecId { get; set; }
        public int isRead { get; set; }
        public int isConfirmed { get; set; }
    
        public virtual Records Records { get; set; }
        public virtual Users Users { get; set; }
    }
}