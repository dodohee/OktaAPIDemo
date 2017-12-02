using System;
using System.Collections.Generic;
using System.Web.Http;
using OktaAPI.Helpers;
using OktaAPIShared.Models;

namespace OktaAPI.Controllers
{
    [CustomerAuthorization]
    public class CustomerController : ApiController {

        // GET: api/Customer
        public IEnumerable<Customer> Get() {
            return APIHelper.GetAllCustomers();
        }

        // GET: api/Customer/5
        public Customer Get(String id)
        {
            return APIHelper.GetCustomerById(id);
        }

        // POST: api/Customer
        //public Customer Post([FromBody]Customer customer)
        //{
        //    return APIHelper.AddNewCustomer(customer);
        //}

        // PUT: api/Customer/5
        //public Customer Put(String id, [FromBody]Customer customer)
        public Customer Put(String id, Customer customer)
        {
            return APIHelper.UpdateCustomer(id, customer);
        }
    }
}
