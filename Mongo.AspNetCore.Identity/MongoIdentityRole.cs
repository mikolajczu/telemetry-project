using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Mongo.AspNetCore.Identity;

public class MongoIdentityRole
{
    public MongoIdentityRole()
    {
        Id = ObjectId.GenerateNewId().ToString();
    }

    public MongoIdentityRole(string name) : this()
    {
        Name = name;
    }
    
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; internal set; }
    
    public string Name { get; internal set; }
    
    public string NormalizedName { get; internal set; }

    public override string ToString() => Name;
}