using RefactorThis.Domain.Enums;
using System.Collections.Generic;
using System.Linq;

namespace RefactorThis.Domain.Entities
{
    public class Invoice
    {
        public decimal Amount { get; set; }
        public decimal AmountPaid { get; set; }
        public decimal TaxAmount { get; set; }
        public List<Payment> Payments { get; set; }
        public InvoiceType Type { get; set; }

        public decimal GetPaymentSum() => HasPayments() ? Payments.Sum(x => x.Amount) : 0.0m;
        public decimal GetRemainingBalance() => Amount - AmountPaid;
        public bool HasPayments() => Payments != null && Payments.Any();
        public bool IsTaxable() => !HasPayments() || Type == InvoiceType.Commercial;
    }
}