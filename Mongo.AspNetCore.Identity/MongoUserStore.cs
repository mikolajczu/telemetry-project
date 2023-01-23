using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using MongoDB.Driver;

namespace Mongo.AspNetCore.Identity;

public class MongoUserStore<TUser> :
    IUserStore<TUser>,
    IUserPasswordStore<TUser>,
    IUserRoleStore<TUser>,
    IUserLoginStore<TUser>,
    IUserSecurityStampStore<TUser>,
    IUserEmailStore<TUser>,
    IUserClaimStore<TUser>,
    IUserPhoneNumberStore<TUser>
    where TUser : MongoIdentityUser
{
    private readonly IMongoCollection<TUser> _usersCollection;

    public MongoUserStore(IMongoCollection<TUser> usersCollection)
    {
        _usersCollection = usersCollection ?? throw new ArgumentNullException(nameof(usersCollection));
    }

    public void Dispose()
    {
        // no need to dispose anything, mongodb driver automatically handles connection pooling
    }

    public virtual async Task<string> GetUserIdAsync(TUser user, CancellationToken cancellationToken = default)
        => await Task.FromResult(user.Id);

    public async Task<string> GetUserNameAsync(TUser user, CancellationToken cancellationToken = default)
        => await Task.FromResult(user.UserName);

    public async Task SetUserNameAsync(TUser user, string userName, CancellationToken cancellationToken = default)
        => await Task.FromResult(user.UserName = userName);

    public async Task<string> GetNormalizedUserNameAsync(TUser user, CancellationToken cancellationToken = default)
        => await Task.FromResult(user.NormalizedUserName);

    public async Task SetNormalizedUserNameAsync(TUser user, string normalizedName,
        CancellationToken cancellationToken = default)
        => await Task.FromResult(user.NormalizedUserName = normalizedName);

    public async Task<IdentityResult> CreateAsync(TUser user, CancellationToken cancellationToken = default)
    {
        await _usersCollection.InsertOneAsync(user, cancellationToken: cancellationToken);

        // TODO result based on action
        return IdentityResult.Success;
    }

    public async Task<IdentityResult> UpdateAsync(TUser user, CancellationToken cancellationToken = default)
    {
        await _usersCollection.ReplaceOneAsync(u => u.Id == user.Id, user, cancellationToken: cancellationToken);

        // TODO result based on action
        return IdentityResult.Success;
    }

    public async Task<IdentityResult> DeleteAsync(TUser user, CancellationToken cancellationToken = default)
    {
        var result = await _usersCollection.DeleteOneAsync(u => u.Id == user.Id, cancellationToken: cancellationToken);

        // TODO result based on action
        return IdentityResult.Success;
    }

    public async Task<TUser> FindByIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        return await _usersCollection.Find(u => u.Id == userId).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<TUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken = default)
    {
        return await _usersCollection.Find(u => u.NormalizedUserName == normalizedUserName)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task SetPasswordHashAsync(TUser user, string passwordHash,
        CancellationToken cancellationToken = default)
        => await Task.FromResult(user.PasswordHash = passwordHash);

    public async Task<string?> GetPasswordHashAsync(TUser user, CancellationToken cancellationToken = default)
        => await Task.FromResult(user.PasswordHash);

    public async Task<bool> HasPasswordAsync(TUser user, CancellationToken cancellationToken = default)
        => await Task.FromResult(user.PasswordHash is not null);

    public async Task AddToRoleAsync(TUser user, string roleName, CancellationToken cancellationToken = default)
    {
        user.AddRole(roleName);
        await Task.CompletedTask;
    }

    public async Task RemoveFromRoleAsync(TUser user, string roleName, CancellationToken cancellationToken)
    {
        user.RemoveRole(roleName);
        await Task.CompletedTask;
    }

    // don't know if i should return copy
    public async Task<IList<string>> GetRolesAsync(TUser user, CancellationToken cancellationToken)
        => await Task.FromResult(user.Roles);

    public async Task<bool> IsInRoleAsync(TUser user, string roleName, CancellationToken cancellationToken)
        => await Task.FromResult(user.Roles.Contains(roleName));

    public async Task<IList<TUser>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
        => await _usersCollection.Find(u => u.Roles.Contains(roleName))
            .ToListAsync(cancellationToken);

    public async Task SetEmailAsync(TUser user, string email, CancellationToken cancellationToken = default)
        => await Task.FromResult(user.Email = email);

    public async Task<string> GetEmailAsync(TUser user, CancellationToken cancellationToken = default)
        => await Task.FromResult(user.Email);

    public async Task<bool> GetEmailConfirmedAsync(TUser user, CancellationToken cancellationToken)
        => await Task.FromResult(user.EmailConfirmed);

    public async Task SetEmailConfirmedAsync(TUser user, bool confirmed, CancellationToken cancellationToken = default)
        => await Task.FromResult(user.EmailConfirmed = confirmed);

    public async Task<TUser> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken = default)
        => await _usersCollection.Find(u => u.NormalizedEmail == normalizedEmail)
            .FirstOrDefaultAsync(cancellationToken);

    public async Task<string> GetNormalizedEmailAsync(TUser user, CancellationToken cancellationToken)
        => await Task.FromResult(user.NormalizedEmail);

    public async Task SetNormalizedEmailAsync(TUser user, string normalizedEmail,
        CancellationToken cancellationToken = default)
        => await Task.FromResult(user.NormalizedEmail = normalizedEmail);

    public async Task SetSecurityStampAsync(TUser user, string stamp, CancellationToken cancellationToken = default)
        => await Task.FromResult(user.SecurityStamp = stamp);

    public async Task<string> GetSecurityStampAsync(TUser user, CancellationToken cancellationToken = default)
        => await Task.FromResult(user.SecurityStamp);

    public async Task AddLoginAsync(TUser user, UserLoginInfo login, CancellationToken cancellationToken = default)
    {
        user.AddLogin(login);
        await Task.CompletedTask;
    }

    public async Task RemoveLoginAsync(TUser user, string loginProvider, string providerKey,
        CancellationToken cancellationToken = default)
    {
        user.RemoveLogin(loginProvider, providerKey);
        await Task.CompletedTask;
    }

    public async Task<IList<UserLoginInfo>> GetLoginsAsync(TUser user, CancellationToken cancellationToken = default)
        => await Task.FromResult(user.Logins.Select(x => x.ToUserLoginInfo()).ToList());

    public async Task<TUser> FindByLoginAsync(string loginProvider, string providerKey,
        CancellationToken cancellationToken = default)
        => await _usersCollection.Find(u =>
                u.Logins.Any(l => l.LoginProvider == loginProvider && l.ProviderKey == providerKey))
            .FirstOrDefaultAsync(cancellationToken);

    public async Task<IList<Claim>> GetClaimsAsync(TUser user, CancellationToken cancellationToken)
        => await Task.FromResult(user.Claims.Select(c => c.ToSecurityClaim()).ToList());

    public async Task AddClaimsAsync(TUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
    {
        claims.ToList().ForEach(user.AddClaim);
        await Task.CompletedTask;
    }

    public async Task ReplaceClaimAsync(TUser user, Claim claim, Claim newClaim,
        CancellationToken cancellationToken = default)
    {
        user.ReplaceClaim(claim, newClaim);
        await Task.CompletedTask;
    }

    public async Task RemoveClaimsAsync(TUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
    {
        claims.ToList().ForEach(user.RemoveClaim);
        await Task.CompletedTask;
    }

    public async Task<IList<TUser>> GetUsersForClaimAsync(Claim claim, CancellationToken cancellationToken)
        => await _usersCollection.Find(u => u.Claims.Any(c => c.Type == claim.Type && c.Value == claim.Value))
            .ToListAsync(cancellationToken);

    public async Task SetPhoneNumberAsync(TUser user, string phoneNumber, CancellationToken cancellationToken = default)
        => await Task.FromResult(user.PhoneNumber = phoneNumber);

    public async Task<string> GetPhoneNumberAsync(TUser user, CancellationToken cancellationToken = default)
        => await Task.FromResult(user.PhoneNumber);

    public async Task<bool> GetPhoneNumberConfirmedAsync(TUser user, CancellationToken cancellationToken = default)
        => await Task.FromResult(user.PhoneNumberConfirmed);

    public async Task SetPhoneNumberConfirmedAsync(TUser user, bool confirmed,
        CancellationToken cancellationToken = default)
        => await Task.FromResult(user.PhoneNumberConfirmed = confirmed);
}