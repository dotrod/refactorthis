using RefactorThis.Domain.Common;
using RefactorThis.Domain.Entities;
using RefactorThis.Domain.Enums;
using RefactorThis.Domain.Repositories;
using System;

namespace RefactorThis.Domain
{
    public class InvoiceService
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private const decimal _taxRate = 0.14m;

        public InvoiceService(IInvoiceRepository invoiceRepository)
        {
            _invoiceRepository = invoiceRepository;
        }

        public string ProcessPayment(Payment payment)
        {
            var invoice = _invoiceRepository.GetInvoice(payment.Reference);
            if (invoice == null)
            {
                throw new InvalidOperationException(InvoiceResponse.NoInvoiceFoundForPaymentReference);
            }

            var invoiceStatus = GetInvoiceStatus(invoice, payment.Amount);
            var paymentResult = ProcessInvoiceByStatus(invoice, payment, invoiceStatus);
            _invoiceRepository.SaveInvoice(invoice);

            return paymentResult;
        }

        private string ProcessInvoiceByStatus(Invoice invoice, Payment payment, InvoiceStatus status)
        {
            switch (status)
            {
                case InvoiceStatus.Invalid:
                    throw new InvalidOperationException(InvoiceResponse.InvalidInvoice);
                case InvoiceStatus.NoPaymentNeeded:
                    return InvoiceResponse.NoPaymentNeeded;
                case InvoiceStatus.Overpaid:
                    return ProcessOverpayment(invoice);
                case InvoiceStatus.Paid:
                    return ProcessPaidInvoice(invoice, payment);
                default:
                    return ProcessPartiallyPaid(invoice, payment);
            }
        }

        private string ProcessPartiallyPaid(Invoice invoice, Payment payment)
        {
            string responseMessage = invoice.HasPayments()
                ? InvoiceResponse.AmountPaidIsLessThanAmountDue
                : InvoiceResponse.AmountPaidIsLessThanInvoiceAmount;

            ApplyPayment(invoice, payment);
            return responseMessage;
        }

        private string ProcessPaidInvoice(Invoice invoice, Payment payment)
        {
            if (!invoice.HasPayments())
            {
                ApplyPayment(invoice, payment);
                return InvoiceResponse.InvoiceNowFullyPaid;
            }

            if (invoice.Amount == invoice.GetPaymentSum())
            {
                return InvoiceResponse.AmountPaidEqualsInvoiceAmount;
            }

            return InvoiceResponse.AmountPaidEqualsAmountDue;
        }

        private void ApplyPayment(Invoice invoice, Payment payment)
        {
            if (invoice.IsTaxable())
            {
                invoice.TaxAmount += payment.Amount * _taxRate;
            }
            invoice.AmountPaid += payment.Amount;
            invoice.Payments.Add(payment);
        }

        private static string ProcessOverpayment(Invoice invoice)
        {
            return invoice.HasPayments()
                ? InvoiceResponse.AmountPaidExceedsAmountDue
                : InvoiceResponse.AmountPaidExceedsInvoiceAmount;
        }

        private static InvoiceStatus GetInvoiceStatus(Invoice invoice, decimal paymentAmount)
        {
            if (invoice.Amount == 0)
            {
                return invoice.HasPayments()
                    ? InvoiceStatus.Invalid
                    : InvoiceStatus.NoPaymentNeeded;
            }

            if (paymentAmount > invoice.GetRemainingBalance())
            {
                return InvoiceStatus.Overpaid;
            }

            return IsPaidInFull(invoice, paymentAmount)
                ? InvoiceStatus.Paid
                : InvoiceStatus.PartiallyPaid;
        }

        private static bool IsPaidInFull(Invoice invoice, decimal paymentAmount)
        {
            return invoice.AmountPaid == invoice.Amount
                || invoice.Amount == invoice.GetPaymentSum()
                || paymentAmount == invoice.GetRemainingBalance();
        }
    }
}