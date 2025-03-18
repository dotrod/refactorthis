
namespace RefactorThis.Domain.Common
{
    public static class InvoiceResponse
    {

        public const string InvalidInvoice = "The invoice is in an invalid state. It has an amount of 0 and it has payments.";
        public const string InvoiceNowFullyPaid = "The invoice is now fully paid.";
        public const string NoInvoiceFoundForPaymentReference = "There is no invoice matching this payment.";
        public const string AmountPaidExceedsInvoiceAmount = "The payment is greater than the invoice amount.";
        public const string AmountPaidIsLessThanInvoiceAmount = "The invoice is now partially paid.";
        public const string NoPaymentNeeded = "No payment needed.";
        public const string AmountPaidEqualsAmountDue = "The final partial payment received. The invoice is now fully paid.";
        public const string AmountPaidEqualsInvoiceAmount = "The invoice was already fully paid.";
        public const string AmountPaidExceedsAmountDue = "The payment is greater than the partial amount remaining.";
        public const string AmountPaidIsLessThanAmountDue = "Another partial payment received, still not fully paid.";
    }
}
