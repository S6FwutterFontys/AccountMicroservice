using System;
using MongoDB.Bson.Serialization.Attributes;

namespace AccountMicroservice.Domain
{
    public class Account
    {
        [BsonId]
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public byte[] Password { get; set; }
        public byte[] Salt { get; set; }
        public string Token { get; set; }
    }
}