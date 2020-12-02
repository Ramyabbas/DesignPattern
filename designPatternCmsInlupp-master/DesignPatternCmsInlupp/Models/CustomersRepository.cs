﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace DesignPatternCmsInlupp.Models
{
    public class CustomersRepository : InterCustomersRepository
    {
        public List<Customer> GetCustomers()
        {
            List<Customer> modell = new List<Customer>();
            string databas = HttpContext.Current.Server.MapPath("~/customers.txt");
            foreach (var line in System.IO.File.ReadAllLines(databas))
            {
                string[] parts = line.Split(';');
                if (parts.Length < 1) continue;
                var customer = new Customer { PersonNummer = parts[0] };
                SetLoansForCustomer(customer);
                SetInvoicesForCustomer(customer);
                SetPaymentsForCustomer(customer);
                modell.Add(customer);

                
            }
            return modell;
        }


        public void SaveCustomers(Customer c)
        {
            string databas = HttpContext.Current.Server.MapPath("~/customers.txt");
            var allLines = System.IO.File.ReadAllLines(databas).ToList();
            foreach (var line in allLines)
            {
                string[] parts = line.Split(';');
                if (parts.Length < 1) continue;
                if (parts[0] == c.PersonNummer)
                    return;
            }
            allLines.Add(c.PersonNummer);
            System.IO.File.WriteAllLines(databas, allLines);
        }

        public void SaveLoan(Customer c, Loan l)
        {
            string databas = HttpContext.Current.Server.MapPath("~/customers.txt");
            var allLines = System.IO.File.ReadAllLines(databas).ToList();
            foreach (var line in allLines)
            {
                string[] parts = line.Split(';');
                if (parts.Length < 1) continue;
                if (parts[0] == c.PersonNummer && parts[1] == l.LoanNo)
                    return;
            }
            allLines.Add($"{c.PersonNummer};{l.LoanNo};{l.Belopp};{l.FromWhen.ToString("yyyy-MM-dd")};{l.InterestRate}");
            System.IO.File.WriteAllLines(databas, allLines);
        }

        public Customer FindCustomer(string personnummer)
        {
            Customer customer = null;
            string databas = HttpContext.Current.Server.MapPath("~/customers.txt");
            foreach (var line in System.IO.File.ReadAllLines(databas))
            {
                string[] parts = line.Split(';');
                if (parts.Length < 1) continue;
                if (parts[0] == personnummer)
                    if (customer == null)
                        customer = new Customer { PersonNummer = personnummer };
            }
            if (customer == null) return null;
            SetLoansForCustomer(customer);
            SetInvoicesForCustomer(customer);
            SetPaymentsForCustomer(customer);
            return customer;
        }

        public void SetLoansForCustomer(Customer c)
        {
            string databas = HttpContext.Current.Server.MapPath("~/customers.txt");
            foreach (var line in System.IO.File.ReadAllLines(databas))
            {
                string[] parts = line.Split(';');
                if (parts.Length < 2) continue;
                if (parts[0] == c.PersonNummer)
                {
                    var loan = new Loan
                    {
                        LoanNo = parts[1],
                        Belopp = Convert.ToInt32(parts[2]),
                        FromWhen = DateTime.ParseExact(parts[3], "yyyy-MM-dd", CultureInfo.InvariantCulture),
                        InterestRate = Convert.ToDecimal(parts[4])
                    };

                    c.Loans.Add(loan);
                }
            }

        }

        public void SetInvoicesForCustomer(Customer customer)
        {
            string databas = HttpContext.Current.Server.MapPath("~/invoices.txt");
            foreach (var line in System.IO.File.ReadAllLines(databas))
            {
                string[] parts = line.Split(';');
                if (parts.Length < 2) continue;
                var loan = customer.Loans.FirstOrDefault(r => r.LoanNo == parts[0]);
                if (loan == null) continue;
                var invoice = new Invoice
                {
                    InvoiceNo = Convert.ToInt32(parts[1]),
                    Belopp = Convert.ToInt32(parts[2]),
                    InvoiceDate = DateTime.ParseExact(parts[3], "yyyy-MM-dd", CultureInfo.InvariantCulture),
                    DueDate = DateTime.ParseExact(parts[3], "yyyy-MM-dd", CultureInfo.InvariantCulture),
                };
                loan.Invoices.Add(invoice);
            }


        }

        public void SetPaymentsForCustomer(Customer customer)
        {
            string databas = HttpContext.Current.Server.MapPath("~/payments.txt");
            foreach (var line in System.IO.File.ReadAllLines(databas))
            {
                string[] parts = line.Split(';');
                if (parts.Length < 2) continue;
                var invoice = customer.Loans.SelectMany(r => r.Invoices).FirstOrDefault(i => i.InvoiceNo == Convert.ToInt32(parts[0]));
                if (invoice == null) continue;
                var payment = new Payment
                {
                    Belopp = Convert.ToInt32(parts[1]),
                    PaymentDate = DateTime.ParseExact(parts[2], "yyyy-MM-dd", CultureInfo.InvariantCulture),
                    BankPaymentReference = parts[3],
                };
                invoice.Payments.Add(payment);
            }


        }
    }
}