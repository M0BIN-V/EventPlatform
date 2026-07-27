using BuildingBlocks.Application;
using OneOf;

namespace Application.UnitTests.Application.Common.Abstractions;

public abstract class HandlerTest<THandler, TRequest, TResponse>
    where THandler : Handler<TRequest, TResponse>
    where TResponse : IOneOf
{
    protected abstract THandler Handler { get; }
    protected abstract IValidator<TRequest> Validator { get; }
    protected abstract TRequest GetRequest();

    [Fact]
    public async Task Handler_WhenRequestIsNotValid_ShouldReturnValidationErrors()
    {
        //Arrange 
        var request = GetRequest();
        var validationResult = new ValidationResult();
        validationResult.Errors.Add(new ValidationFailure());

        Validator.ValidateAsync(Any<TRequest>(), Any<CancellationToken>())
            .Returns(validationResult);

        //Act 
        var result = await Handler.HandleAsync(request, CancellationToken.None);

        //Assert 
        result.Value.ShouldBeOfType<List<ValidationFailure>>();
    }
}