namespace Identity.Application.Common.Contracts.Services;

public interface ISecureTokenGenerator
{
    string Generate();
}