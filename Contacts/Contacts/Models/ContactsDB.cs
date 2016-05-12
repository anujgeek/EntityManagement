namespace Contacts.Models
{
    using Common;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity;
    using System.Linq;

    public partial class ContactsDB : DbContext
    {
        public ContactsDB()
            : base(Utilities.GetConnectionStringForDatabase())
        {
        }

        public virtual DbSet<Contact> Contacts { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
        }
    }
}
