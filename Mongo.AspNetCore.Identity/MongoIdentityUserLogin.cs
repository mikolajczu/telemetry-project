using Microsoft.AspNetCore.Identity;

namespace Mongo.AspNetCore.Identity;

public class MongoIdentityUserLogin
{
    public string LoginProvider { get; set; }
    public string ProviderDisplayName { get; set; }
    public string ProviderKey { get; set; }
    
    public MongoIdentityUserLogin(string loginProvider, string providerDisplayName, string providerKey)
    {
        LoginProvider = loginProvider;
        ProviderDisplayName = providerDisplayName;
        ProviderKey = providerKey;
    }

    public MongoIdentityUserLogin(UserLoginInfo userLoginInfo)
    {
        LoginProvider = userLoginInfo.LoginProvider;
        ProviderDisplayName = userLoginInfo.ProviderDisplayName;
        ProviderKey = userLoginInfo.ProviderKey;
    }

    public virtual UserLoginInfo ToUserLoginInfo()
        => new UserLoginInfo(LoginProvider, ProviderKey, ProviderDisplayName);
}