namespace Contacts.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Contact
    {
        public int id { get; set; }

        [Required]
        [StringLength(50)]
        public string first_name { get; set; }

        [StringLength(50)]
        public string last_name { get; set; }

        [StringLength(50)]
        public string company_name { get; set; }

        [StringLength(255)]
        public string address { get; set; }

        [StringLength(50)]
        public string city { get; set; }

        [StringLength(50)]
        public string state { get; set; }

        [StringLength(50)]
        public string zip { get; set; }

        [StringLength(50)]
        public string phone { get; set; }

        [StringLength(50)]
        public string work_phone { get; set; }

        [StringLength(50)]
        public string email { get; set; }

        [StringLength(50)]
        public string url { get; set; }

        [Column(TypeName = "date")]
        public DateTime? dob { get; set; }
    }
}
