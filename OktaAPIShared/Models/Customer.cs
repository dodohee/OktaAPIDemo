using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace OktaAPIShared.Models
{

    [DataContract]
    public class Customer
    {
        [DataMember(Name = "id")]
        public String Id { get; set; }

        [DataMember(Name = "status")]
        public String Status { get; set; }

        [DataMember(Name = "profile")]
        public Profile Profile { get; set; }

        public Customer()
        {
            Profile = new Profile();
        }
    }

    [DataContract]
    public class AddCustomer
    {
        [DataMember(Name = "profile")]
        public Profile Profile { get; set; }

        [DataMember(Name = "credentials")]
        public Credentials Credentials { get; set; }

        public AddCustomer()
        {
            Credentials = new Credentials();
        }
    }

    [DataContract]
    public class Credentials
    {
        [Required]
        [DataType(DataType.Password)]
        [StringLength(20, MinimumLength = 8)]
        [DataMember(Name = "password")]
        public string Password { get; set; }
    }

}