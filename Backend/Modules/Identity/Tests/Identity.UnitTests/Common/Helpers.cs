using Identity.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Application.UnitTests.Common;

public class Helpers
{
    public static UserManager<User> CreateUserManager()
    {
        var store = For<IUserStore<User>>();
        var identityOptions = For<IOptions<IdentityOptions>>();
        identityOptions.Value.Returns(new IdentityOptions());
        var pwdHasher = For<IPasswordHasher<User>>();
        var userValidators = new List<IUserValidator<User>>();
        var pwdValidators = new List<IPasswordValidator<User>>();
        var normalizer = For<ILookupNormalizer>();
        var describer = For<IdentityErrorDescriber>();
        var services = For<IServiceProvider>();
        var logger = For<ILogger<UserManager<User>>>();

        return For<UserManager<User>>(store, identityOptions, pwdHasher, userValidators, pwdValidators,
            normalizer,
            describer, services, logger);
    }
}