using Microsoft.AspNetCore.Identity;
using MongoDB.Driver;

namespace Mongo.AspNetCore.Identity;

public class MongoRoleStore<TRole> : IRoleStore<TRole> 
    where TRole : MongoIdentityRole
{
    private readonly IMongoCollection<TRole> _rolesCollection;

    public MongoRoleStore(IMongoCollection<TRole> rolesCollection)
    {
        _rolesCollection = rolesCollection;
    }
    
    public void Dispose()
    {
        // no need to dispose anything, mongodb driver automatically handles connection pooling
    }

    public async Task<IdentityResult> CreateAsync(TRole role, CancellationToken cancellationToken = default)
    {
        await _rolesCollection.InsertOneAsync(role, cancellationToken: cancellationToken);
        
        return IdentityResult.Success;
    }

    public async Task<IdentityResult> UpdateAsync(TRole role, CancellationToken cancellationToken = default)
    {
        await _rolesCollection.ReplaceOneAsync(r => r.Id == role.Id, role, cancellationToken: cancellationToken);

        return IdentityResult.Success;
    }

    public async Task<IdentityResult> DeleteAsync(TRole role, CancellationToken cancellationToken = default)
    {
        await _rolesCollection.DeleteOneAsync(r => r.Id == role.Id, cancellationToken: cancellationToken);
        
        return IdentityResult.Success;
    }

    public async Task<string> GetRoleIdAsync(TRole role, CancellationToken cancellationToken = default)
        => await Task.FromResult(role.Id);

    public async Task<string> GetRoleNameAsync(TRole role, CancellationToken cancellationToken = default)
        => await Task.FromResult(role.Name);

    public async Task SetRoleNameAsync(TRole role, string roleName, CancellationToken cancellationToken = default)
        => await Task.FromResult(role.Name = roleName);

    public async Task<string> GetNormalizedRoleNameAsync(TRole role, CancellationToken cancellationToken = default)
        => await Task.FromResult(role.NormalizedName);

    public async Task SetNormalizedRoleNameAsync(TRole role, string normalizedName, CancellationToken cancellationToken = default)
        => await Task.FromResult(role.NormalizedName = normalizedName);

    public async Task<TRole> FindByIdAsync(string roleId, CancellationToken cancellationToken = default)
    {
        return await _rolesCollection.Find(r => r.Id == roleId).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<TRole> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken = default)
    {
        return await _rolesCollection.Find(r => r.NormalizedName == normalizedRoleName)
            .FirstOrDefaultAsync(cancellationToken);
    }
}