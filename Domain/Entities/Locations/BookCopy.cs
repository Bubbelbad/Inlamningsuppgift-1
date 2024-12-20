﻿using Domain.Entities.Core;
using Domain.Entities.Transactions;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities.Locations
{
    public class BookCopy
    {
        [Key]
        public Guid CopyId { get; set; }
        public Guid BookId { get; set; }
        public int BranchId { get; set; }
        public string? Status { get; set; }

        // Navigation properties
        public Book Book { get; set; }
        public LibraryBranch Branch { get; set; }
        public ICollection<Borrowing> Borrowings { get; set; }
        public ICollection<Reservation> Reservations { get; set; }
    }

}
