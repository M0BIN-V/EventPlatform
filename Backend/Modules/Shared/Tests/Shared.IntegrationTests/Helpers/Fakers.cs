using Bogus;
using Identity.Application.Features.Register;

namespace Shared.IntegrationTests.Helpers;

public class Fakers
{
    public static Faker<RegisterRequest> RegisterRequestFaker { get; } = new Faker<RegisterRequest>()
        .CustomInstantiator(f => new RegisterRequest(
            f.Person.FirstName,
            f.Person.LastName,
            f.Person.Email,
            f.Internet.Password(8, false, null, "Aa1!")));
}