namespace RefactorThis.Domain.Enums
{
    internal enum InvoiceStatus
    {
        Invalid,
        NoPaymentNeeded,
        Paid,
        PartiallyPaid,
        Overpaid
    }
}