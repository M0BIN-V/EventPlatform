using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Application.UnitTests.Common;

public class FakeUserManagerBuilder
{
    public readonly IdentityErrorDescriber Describer = For<IdentityErrorDescriber>();
    public readonly IOptions<IdentityOptions> IdentityOptions = For<IOptions<IdentityOptions>>();
    public readonly ILogger<UserManager<User>> Logger = For<ILogger<UserManager<User>>>();
    public readonly ILookupNormalizer Normalizer = For<ILookupNormalizer>();
    public readonly IPasswordHasher<User> PwdHasher = For<IPasswordHasher<User>>();
    public readonly IEnumerable<IPasswordValidator<User>> PwdValidators = new List<IPasswordValidator<User>>();
    public readonly IServiceProvider Services = For<IServiceProvider>();
    public readonly IUserStore<User> UserStore = For<IUserStore<User>>();
    public readonly IEnumerable<IUserValidator<User>> UserValidators = new List<IUserValidator<User>>();

    public UserManager<User> Create()
    {
        IdentityOptions.Value.Returns(new IdentityOptions());
        
        return For<UserManager<User>>(UserStore, IdentityOptions, PwdHasher, UserValidators, PwdValidators,
            Normalizer,
            Describer, Services, Logger);
    }
}