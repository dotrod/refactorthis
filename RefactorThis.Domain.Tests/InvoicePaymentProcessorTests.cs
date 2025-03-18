using Moq;
using NUnit.Framework;
using RefactorThis.Domain.Common;
using RefactorThis.Domain.Entities;
using RefactorThis.Domain.Repositories;
using System;
using System.Collections.Generic;

namespace RefactorThis.Domain.Tests
{
    [TestFixture]
    public class InvoicePaymentProcessorTests
    {
        private Mock<IInvoiceRepository> _mockRepo;
        private InvoiceService _invoiceService;

        [SetUp]
        public void SetUp()
        {
            _mockRepo = new Mock<IInvoiceRepository>();
            _invoiceService  = new InvoiceService(_mockRepo.Object);
        }

        [Test]
        public void ProcessPayment_Should_ThrowException_When_NoInvoiceFoundForPaymentReference()
        {
            var payment = new Payment();
            _mockRepo.Setup(repo => repo.GetInvoice(It.IsAny<string>())).Returns((Invoice)null);

            var exception = Assert.Throws<InvalidOperationException>(() => _invoiceService.ProcessPayment(payment));

            Assert.AreEqual(InvoiceResponse.NoInvoiceFoundForPaymentReference, exception.Message);
        }

        [Test]
        public void ProcessPayment_Should_ReturnFailureMessage_When_NoPaymentNeeded()
        {
            var invoice = new Invoice()
            {
                Amount = 0,
                AmountPaid = 0,
                Payments = null
            };
            var payment = new Payment();
            _mockRepo.Setup(repo => repo.GetInvoice(It.IsAny<string>())).Returns(invoice);

            var response = _invoiceService.ProcessPayment(payment);

            Assert.AreEqual(InvoiceResponse.NoPaymentNeeded, response);
        }

        [Test]
        public void ProcessPayment_Should_ReturnFailureMessage_When_InvoiceAlreadyFullyPaid()
        {
            var invoice = new Invoice()
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
            _mockRepo.Setup(repo => repo.GetInvoice(It.IsAny<string>())).Returns(invoice);

            var payment = new Payment();
            var response = _invoiceService.ProcessPayment(payment);

            Assert.AreEqual(InvoiceResponse.AmountPaidEqualsInvoiceAmount, response);
        }

        [Test]
        public void ProcessPayment_Should_ReturnFailureMessage_When_PartialPaymentExistsAndAmountPaidExceedsAmountDue()
        {
            var invoice = new Invoice()
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
            var payment = new Payment()
            {
                Amount = 6
            };
            _mockRepo.Setup(repo => repo.GetInvoice(It.IsAny<string>())).Returns(invoice);

            var response = _invoiceService.ProcessPayment(payment);

            Assert.AreEqual(InvoiceResponse.AmountPaidExceedsAmountDue, response);
        }

        [Test]
        public void ProcessPayment_Should_ReturnFailureMessage_When_NoPartialPaymentExistsAndAmountPaidExceedsInvoiceAmount()
        {
            var invoice = new Invoice()
            {
                Amount = 5,
                AmountPaid = 0,
                Payments = new List<Payment>()
            };
            var payment = new Payment()
            {
                Amount = 6
            };
            _mockRepo.Setup(repo => repo.GetInvoice(It.IsAny<string>())).Returns(invoice);

            var result = _invoiceService.ProcessPayment(payment);

            Assert.AreEqual(InvoiceResponse.AmountPaidExceedsInvoiceAmount, result);
        }

        [Test]
        public void ProcessPayment_Should_ReturnFullyPaidMessage_When_PartialPaymentExistsAndAmountPaidEqualsAmountDue()
        {
            var invoice = new Invoice()
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
            var payment = new Payment()
            {
                Amount = 5
            };
            _mockRepo.Setup(repo => repo.GetInvoice(It.IsAny<string>())).Returns(invoice);

            var result = _invoiceService.ProcessPayment(payment);

            Assert.AreEqual(InvoiceResponse.AmountPaidEqualsAmountDue, result);
        }

        [Test]
        public void ProcessPayment_Should_ReturnFullyPaidMessage_When_PartialPaymentExistsAndAmountPaidEqualsInvoiceAmount()
        {
            var invoice = new Invoice()
            {
                Amount = 10,
                AmountPaid = 0,
                Payments = new List<Payment>()
                {
                    new Payment()
                    {
                        Amount = 10
                    }
                }
            };
            var payment = new Payment()
            {
                Amount = 10
            };
            _mockRepo.Setup(repo => repo.GetInvoice(It.IsAny<string>())).Returns(invoice);

            var result = _invoiceService.ProcessPayment(payment);

            Assert.AreEqual(InvoiceResponse.AmountPaidEqualsInvoiceAmount, result);
        }

        [Test]
        public void ProcessPayment_Should_ReturnPartiallyPaidMessage_When_PartialPaymentExistsAndAmountPaidIsLessThanAmountDue()
        {
            var invoice = new Invoice()
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
            var payment = new Payment()
            {
                Amount = 1
            };
            _mockRepo.Setup(repo => repo.GetInvoice(It.IsAny<string>())).Returns(invoice);

            var result = _invoiceService.ProcessPayment(payment);

            Assert.AreEqual(InvoiceResponse.AmountPaidIsLessThanAmountDue, result);
        }

        [Test]
        public void ProcessPayment_Should_ReturnPartiallyPaidMessage_When_NoPartialPaymentExistsAndAmountPaidIsLessThanInvoiceAmount()
        {
            var invoice = new Invoice()
            {
                Amount = 10,
                AmountPaid = 0,
                Payments = new List<Payment>()
            };
            var payment = new Payment()
            {
                Amount = 1
            };
            _mockRepo.Setup(repo => repo.GetInvoice(It.IsAny<string>())).Returns(invoice);

            var result = _invoiceService.ProcessPayment(payment);

            Assert.AreEqual(InvoiceResponse.AmountPaidIsLessThanInvoiceAmount, result);
        }
    }
}