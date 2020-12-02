using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatternCmsInlupp.Models
{
    public interface InterCustomersRepository
    {
        List<Customer> GetCustomers();
        Customer FindCustomer(string socialSecurityNumber);
        void SaveLoan(Customer customer, Loan loan);
        void SaveCustomers(Customer customer);
        
        
    }
}
