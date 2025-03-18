using NUnit.Framework;
using RefactorThis.Domain.Common;
using RefactorThis.Persistence;
using System;
using System.Collections.Generic;

namespace RefactorThis.Domain.Tests
{
    [TestFixture]
    public class InvoicePaymentProcessorTests
    {
        [Test]
        public void ProcessPayment_Should_ThrowException_When_NoInvoiceFoundForPaymentReference()
        {
            var repo = new InvoiceRepository();

            Invoice invoice = null;
            var paymentProcessor = new InvoiceService(repo);

            var payment = new Payment();
            var failureMessage = "";

            try
            {
                var result = paymentProcessor.ProcessPayment(payment);
            }
            catch (InvalidOperationException e)
            {
                failureMessage = e.Message;
            }

            Assert.AreEqual(InvoiceResponse.NoInvoiceFoundForPaymentReference, failureMessage);
        }

        [Test]
        public void ProcessPayment_Should_ReturnFailureMessage_When_NoPaymentNeeded()
        {
            var repo = new InvoiceRepository();

            var invoice = new Invoice(repo)
            {
                Amount = 0,
                AmountPaid = 0,
                Payments = null
            };

            repo.Add(invoice);

            var paymentProcessor = new InvoiceService(repo);

            var payment = new Payment();

            var result = paymentProcessor.ProcessPayment(payment);

            Assert.AreEqual(InvoiceResponse.NoPaymentNeeded, result);
        }

        [Test]
        public void ProcessPayment_Should_ReturnFailureMessage_When_InvoiceAlreadyFullyPaid()
        {
            var repo = new InvoiceRepository();

            var invoice = new Invoice(repo)
            {
                Amount = 10,
                AmountPaid = 10,
                Payments = new List<Payment>
                {
                    new Payment
                    {
                        Amount = 10
                    }
                }
            };
            repo.Add(invoice);

            var paymentProcessor = new InvoiceService(repo);

            var payment = new Payment();

            var result = paymentProcessor.ProcessPayment(payment);

            Assert.AreEqual(InvoiceResponse.AmountPaidEqualsInvoiceAmount, result);
        }

        [Test]
        public void ProcessPayment_Should_ReturnFailureMessage_When_PartialPaymentExistsAndAmountPaidExceedsAmountDue()
        {
            var repo = new InvoiceRepository();
            var invoice = new Invoice(repo)
            {
                Amount = 10,
                AmountPaid = 5,
                Payments = new List<Payment>
                {
                    new Payment
                    {
                        Amount = 5
                    }
                }
            };
            repo.Add(invoice);

            var paymentProcessor = new InvoiceService(repo);

            var payment = new Payment()
            {
                Amount = 6
            };

            var result = paymentProcessor.ProcessPayment(payment);

            Assert.AreEqual(InvoiceResponse.AmountPaidExceedsAmountDue, result);
        }

        [Test]
        public void ProcessPayment_Should_ReturnFailureMessage_When_NoPartialPaymentExistsAndAmountPaidExceedsInvoiceAmount()
        {
            var repo = new InvoiceRepository();
            var invoice = new Invoice(repo)
            {
                Amount = 5,
                AmountPaid = 0,
                Payments = new List<Payment>()
            };
            repo.Add(invoice);

            var paymentProcessor = new InvoiceService(repo);

            var payment = new Payment()
            {
                Amount = 6
            };

            var result = paymentProcessor.ProcessPayment(payment);

            Assert.AreEqual(InvoiceResponse.AmountPaidExceedsInvoiceAmount, result);
        }

        [Test]
        public void ProcessPayment_Should_ReturnFullyPaidMessage_When_PartialPaymentExistsAndAmountPaidEqualsAmountDue()
        {
            var repo = new InvoiceRepository();
            var invoice = new Invoice(repo)
            {
                Amount = 10,
                AmountPaid = 5,
                Payments = new List<Payment>
                {
                    new Payment
                    {
                        Amount = 5
                    }
                }
            };
            repo.Add(invoice);

            var paymentProcessor = new InvoiceService(repo);

            var payment = new Payment()
            {
                Amount = 5
            };

            var result = paymentProcessor.ProcessPayment(payment);

            Assert.AreEqual(InvoiceResponse.AmountPaidEqualsAmountDue, result);
        }

        [Test]
        public void ProcessPayment_Should_ReturnFullyPaidMessage_When_NoPartialPaymentExistsAndAmountPaidEqualsInvoiceAmount()
        {
            var repo = new InvoiceRepository();
            var invoice = new Invoice(repo)
            {
                Amount = 10,
                AmountPaid = 0,
                Payments = new List<Payment>() { new Payment() { Amount = 10 } }
            };
            repo.Add(invoice);

            var paymentProcessor = new InvoiceService(repo);

            var payment = new Payment()
            {
                Amount = 10
            };

            var result = paymentProcessor.ProcessPayment(payment);

            Assert.AreEqual(InvoiceResponse.AmountPaidEqualsInvoiceAmount, result);
        }

        [Test]
        public void ProcessPayment_Should_ReturnPartiallyPaidMessage_When_PartialPaymentExistsAndAmountPaidIsLessThanAmountDue()
        {
            var repo = new InvoiceRepository();
            var invoice = new Invoice(repo)
            {
                Amount = 10,
                AmountPaid = 5,
                Payments = new List<Payment>
                {
                    new Payment
                    {
                        Amount = 5
                    }
                }
            };
            repo.Add(invoice);

            var paymentProcessor = new InvoiceService(repo);

            var payment = new Payment()
            {
                Amount = 1
            };

            var result = paymentProcessor.ProcessPayment(payment);

            Assert.AreEqual(InvoiceResponse.AmountPaidIsLessThanAmountDue, result);
        }

        [Test]
        public void ProcessPayment_Should_ReturnPartiallyPaidMessage_When_NoPartialPaymentExistsAndAmountPaidIsLessThanInvoiceAmount()
        {
            var repo = new InvoiceRepository();
            var invoice = new Invoice(repo)
            {
                Amount = 10,
                AmountPaid = 0,
                Payments = new List<Payment>()
            };
            repo.Add(invoice);

            var paymentProcessor = new InvoiceService(repo);

            var payment = new Payment()
            {
                Amount = 1
            };

            var result = paymentProcessor.ProcessPayment(payment);

            Assert.AreEqual(InvoiceResponse.AmountPaidIsLessThanInvoiceAmount, result);
        }
    }
}