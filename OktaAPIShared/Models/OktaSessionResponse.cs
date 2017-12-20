using System;
using System.Runtime.Serialization;

namespace OktaAPIShared.Models {
    [DataContract]
    public class OktaSessionResponse
    {
        [DataMember(Name = "expiresAt")]
        public string expiresAt { get; set; }

        [DataMember(Name = "status")]
        public string status { get; set; }

        [DataMember(Name = "sessionToken")]
        public string SessionToken { get; set; }

        [DataMember(Name = "_embedded")]
        public _embedded _embedded { get; set; }

        [DataMember(Name = "errorCode")]
        public _embedded errorCode { get; set; }

        [DataMember(Name = "errorSummary")]
        public _embedded errorSummary { get; set; }
    }

    [DataContract]
    public class _embedded
    {
        public user user { get; set; }
    }

    [DataContract]
    public class user
    {
        public string id { get; set; }
        public string passwordChanged { get; set; }

        public profile profile { get; set; }
    }

    [DataContract]
    public class profile
    {
        public string login { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
    }
}
