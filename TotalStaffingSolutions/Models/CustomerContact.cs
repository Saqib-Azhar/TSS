//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TotalStaffingSolutions.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class CustomerContact
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public CustomerContact()
        {
            this.ContactConfirmations = new HashSet<ContactConfirmation>();
        }
    
        public int Id { get; set; }
        public string Contact_name { get; set; }
        public string Contact_code { get; set; }
        public string Contact_description { get; set; }
        public string Contact_key { get; set; }
        public string Customer_key { get; set; }
        public string Email_id { get; set; }
        public string Phone_1 { get; set; }
        public string Phone_2 { get; set; }
        public string Phone_3 { get; set; }
        public string Contact_notes { get; set; }
        public string Customer_id { get; set; }
        public Nullable<System.DateTime> ENTITY_ADDED_AT { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ContactConfirmation> ContactConfirmations { get; set; }
    }
}
