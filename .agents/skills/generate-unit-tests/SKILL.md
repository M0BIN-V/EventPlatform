# Unit Test Guidelines

## Purpose

Generate unit tests that are consistent with the existing codebase.

Prioritize consistency over creativity. Reuse existing patterns, helpers, builders, naming conventions, and assertion style instead of introducing new ones.

---

# Project Structure

Place tests under:

Backend/Modules/<Module>/Tests/<Module>.UnitTests

Group tests by feature:

Features/<Feature>/<Feature>HandlerUnitTests.cs

---

# Naming

Test classes:

<HandlerName>UnitTests

Test methods:

Handler_When<Condition>_Should<ExpectedResult>

Examples:

Handler_WhenRequestIsValid_ShouldReturnSuccess()

Handler_WhenUserDoesNotExist_ShouldReturnNotFound()

Handler_WhenValidationFails_ShouldReturnValidationErrors()

Use xUnit `[Fact]` for individual test cases.

---

# Test Layout

Use the Arrange / Act / Assert pattern with comments separating each section.

```csharp
// Arrange

// Act

// Assert
```

Each test should verify a single behavior.

Avoid combining multiple unrelated scenarios in one test.

---

# Base Classes & Helpers

Always reuse existing test infrastructure.

Prefer:

- HandlerTest<THandler, TRequest, TResponse>
- FakeUserManagerBuilder
- For<T>()
- Any<T>()

Do not recreate helpers that already exist.

---

# Mocking

Use NSubstitute APIs.

Examples:

- Returns(...)
- Received(...)
- DidNotReceive()

Never use:

- real databases
- file system
- network calls
- external services

Configure `IOptions<T>` by mocking `.Value.Returns(...)`.

Only configure dependencies required by the current test.

Avoid unnecessary setup copied from other tests.

---

# Requests & Domain Objects

Always create real instances for:

- Requests
- Commands
- Queries
- DTOs
- Entities
- Value Objects

Good:

```csharp
var request = new RegisterRequest(...);
```

Avoid:

```csharp
var request = Substitute.For<RegisterRequest>();
```

Do not mock domain entities unless absolutely necessary.

---

# Validation

Handler tests verify handler behavior.

Validator tests verify validation rules.

Do not duplicate FluentValidation rules inside handler tests.

Unless a test specifically targets validation failures, configure the validator to return a successful ValidationResult.

Example:

```csharp
_validator
    .ValidateAsync(request, Any<CancellationToken>())
    .Returns(new ValidationResult());
```

---

# Assertions

Use Shouldly assertions.

Prefer:

- ShouldBe()
- ShouldNotBeNull()
- ShouldBeOfType()

Assert:

- returned result
- response value
- business side-effects

Avoid asserting implementation details.

---

# Interaction Verification

Verify only meaningful business interactions.

Good:

```csharp
publisher.Received(1).PublishAsync(...);

repository.Received(1).Add(...);
```

Avoid verifying every dependency call.

Only verify interactions that represent business behavior.

---

# OneOf Responses

When handlers return OneOf responses, assert the concrete returned type.

Example:

```csharp
result.Value.ShouldBeOfType<SuccessResponse>();

result.Value.ShouldBeOfType<ValidationErrors>();

result.Value.ShouldBeOfType<UserNotFoundError>();
```

---

# Common Patterns

Use existing tests (such as RegisterHandlerUnitTests) as the reference implementation.

Default assumptions:

- validator succeeds
- repository succeeds
- UnitOfWork succeeds
- external services succeed

Override only what is necessary for the scenario being tested.

Common scenarios:

- Validation failure
- Entity not found
- Business rule violation
- External dependency failure
- Successful execution

---

# Do

- Reuse existing helpers.
- Keep tests isolated.
- Keep tests deterministic.
- Write small focused tests.
- Use meaningful test names.
- Follow the project's existing style.

---

# Don't

- Don't access databases.
- Don't call external services.
- Don't test framework internals.
- Don't mock DTOs or entities unnecessarily.
- Don't verify every dependency interaction.
- Don't introduce new testing patterns when an existing one already exists.

---

# Running Tests

Run:

```bash
dotnet test
```

or

```bash
dotnet test Backend/Modules/Identity/Tests/Identity.UnitTests/Identity.UnitTests.csproj
```

---

# Pull Request Checklist

- Tests compile.
- Tests pass.
- Naming conventions are respected.
- No external dependencies.
- Happy path is covered.
- Failure paths are covered.
- Existing project conventions are followed.

---

# Golden Rule

When generating tests:

1. Match the existing codebase style.
2. Prefer reuse over reimplementation.
3. Test observable behavior, not implementation details.
4. Keep tests simple, deterministic, and focused.