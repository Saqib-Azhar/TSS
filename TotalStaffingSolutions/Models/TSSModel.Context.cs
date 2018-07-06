﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class TSS_Sql_Entities : DbContext
    {
        public TSS_Sql_Entities()
            : base("name=TSS_Sql_Entities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<C__MigrationHistory> C__MigrationHistory { get; set; }
        public virtual DbSet<AspNetRole> AspNetRoles { get; set; }
        public virtual DbSet<AspNetUserClaim> AspNetUserClaims { get; set; }
        public virtual DbSet<AspNetUserLogin> AspNetUserLogins { get; set; }
        public virtual DbSet<AspNetUser> AspNetUsers { get; set; }
        public virtual DbSet<Branch> Branches { get; set; }
        public virtual DbSet<Company> Companies { get; set; }
        public virtual DbSet<Company_Emails> Company_Emails { get; set; }
        public virtual DbSet<ContactConfirmation> ContactConfirmations { get; set; }
        public virtual DbSet<ContactConfirmationStatu> ContactConfirmationStatus { get; set; }
        public virtual DbSet<CustomerContact> CustomerContacts { get; set; }
        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<Employee> Employees { get; set; }
        public virtual DbSet<Organization> Organizations { get; set; }
        public virtual DbSet<Timesheet_details> Timesheet_details { get; set; }
        public virtual DbSet<Timesheet_summaries> Timesheet_summaries { get; set; }
        public virtual DbSet<Timesheet> Timesheets { get; set; }
        public virtual DbSet<TimeSheetStatu> TimeSheetStatus { get; set; }
        public virtual DbSet<User_Branches> User_Branches { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<AggregatedCounter> AggregatedCounters { get; set; }
        public virtual DbSet<Counter> Counters { get; set; }
        public virtual DbSet<Hash> Hashes { get; set; }
        public virtual DbSet<Job> Jobs { get; set; }
        public virtual DbSet<JobParameter> JobParameters { get; set; }
        public virtual DbSet<JobQueue> JobQueues { get; set; }
        public virtual DbSet<List> Lists { get; set; }
        public virtual DbSet<Schema> Schemata { get; set; }
        public virtual DbSet<Server> Servers { get; set; }
        public virtual DbSet<Set> Sets { get; set; }
        public virtual DbSet<State> States { get; set; }
    }
}
