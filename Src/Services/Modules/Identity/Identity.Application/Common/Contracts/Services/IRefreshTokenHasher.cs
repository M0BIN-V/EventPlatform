namespace Identity.Application.Common.Contracts.Services;


public interface IRefreshTokenHasher
{
  
    string HashToken(string token);
}
