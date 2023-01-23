using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Mongo.AspNetCore.Identity;

public class MongoIdentityUser
{
    /// <summary>
    /// Constructor which creates a new ObjectId for the Id.
    /// </summary>
    public MongoIdentityUser()
    {
        Id = ObjectId.GenerateNewId().ToString();
        Roles = new List<string>();
        Claims = new List<MongoIdentityUserClaim>();
        Logins = new List<MongoIdentityUserLogin>();
    }
    
    /// <summary>
    /// Constructor that takes a userName.
    /// </summary>
    /// <param name="userName"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public MongoIdentityUser(string userName) : this()
    {
        UserName = userName ?? throw new ArgumentNullException(nameof(userName));
    }
    
    [BsonRepresentation(BsonType.ObjectId)]
    public virtual string Id { get; internal set; }
    
    public virtual string UserName { get; internal set; }
    
    public virtual string NormalizedUserName { get; internal set; }

    public virtual string Email { get; internal set; }
    
    public virtual string SecurityStamp { get; internal set; }

    public virtual bool EmailConfirmed { get; set; }
    
    public virtual string NormalizedEmail { get; internal set; }

    public virtual List<string> Roles { get; internal set; }
    
    public virtual List<MongoIdentityUserLogin> Logins { get; internal set; }

    public virtual void AddLogin(UserLoginInfo userLoginInfo)
        => Logins.Add(new MongoIdentityUserLogin(userLoginInfo));

    public virtual void RemoveLogin(string loginProvider, string providerKey)
        => Logins.RemoveAll(l =>
            l.LoginProvider == loginProvider && l.ProviderKey == providerKey);

    public virtual void AddRole(string role)
        => Roles.Add(role);

    public virtual void RemoveRole(string role)
        => Roles.Remove(role);

    public virtual List<MongoIdentityUserClaim> Claims { get; internal set; }

    public virtual void AddClaim(Claim claim)
        => Claims.Add(new MongoIdentityUserClaim(claim));

    public virtual void RemoveClaim(Claim claim)
    {
        Claims.RemoveAll(c => c.Type == claim.Type && c.Value == claim.Type);
    }

    public virtual void ReplaceClaim(Claim existingClaim, Claim newClaim)
    {
        var claimExists = Claims.Any(c => c.Type == existingClaim.Type && c.Value == existingClaim.Value);

        if (!claimExists)
            return;

        RemoveClaim(existingClaim);
        AddClaim(newClaim);
    }

    public virtual string? PasswordHash { get; set; }
    
    public virtual string PhoneNumber { get; internal set; }
    
    public virtual bool PhoneNumberConfirmed { get; internal set; }
}