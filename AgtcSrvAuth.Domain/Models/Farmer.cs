namespace AgtcSrvAuth.Domain.Models;

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class Farmer
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }    
}