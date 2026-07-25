namespace Identity.Domain.Constants;

public enum RevocationReason
{
    ReuseAttack = 1,
    Logout = 2,
    Rotated = 3
}