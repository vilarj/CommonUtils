using System.Security.Claims;

namespace CommonUtils.Auth;

/// <summary>
/// Extension methods on <see cref="ClaimsPrincipal"/> for the claim types that appear
/// in every API's JWT payload, eliminating repeated <c>FindFirst</c> boilerplate.
/// </summary>
public static class ClaimsPrincipalExtensions
{
    /// <summary>
    /// Returns the user's ID from the <see cref="ClaimTypes.NameIdentifier"/> claim,
    /// or <c>null</c> if the claim is absent.
    /// </summary>
    public static string? GetUserId(this ClaimsPrincipal principal)
    {
        ArgumentNullException.ThrowIfNull(principal);
        return principal.FindFirstValue(ClaimTypes.NameIdentifier);
    }

    /// <summary>
    /// Returns the user's email address from the <see cref="ClaimTypes.Email"/> claim,
    /// or <c>null</c> if the claim is absent.
    /// </summary>
    public static string? GetEmail(this ClaimsPrincipal principal)
    {
        ArgumentNullException.ThrowIfNull(principal);
        return principal.FindFirstValue(ClaimTypes.Email);
    }

    /// <summary>
    /// Returns the user's role from the <see cref="ClaimTypes.Role"/> claim,
    /// or <c>null</c> if the claim is absent. When multiple role claims are
    /// present this returns the first one.
    /// </summary>
    public static string? GetRole(this ClaimsPrincipal principal)
    {
        ArgumentNullException.ThrowIfNull(principal);
        return principal.FindFirstValue(ClaimTypes.Role);
    }

    /// <summary>
    /// Returns all role claims on the principal.
    /// </summary>
    public static IEnumerable<string> GetRoles(this ClaimsPrincipal principal)
    {
        ArgumentNullException.ThrowIfNull(principal);
        return principal.FindAll(ClaimTypes.Role).Select(c => c.Value);
    }

    /// <summary>
    /// Returns the value of a claim by <paramref name="claimType"/>,
    /// or <c>null</c> if the claim is absent.
    /// </summary>
    public static string? GetClaim(this ClaimsPrincipal principal, string claimType)
    {
        ArgumentNullException.ThrowIfNull(principal);
        ArgumentException.ThrowIfNullOrWhiteSpace(claimType);
        return principal.FindFirstValue(claimType);
    }

    /// <summary>
    /// Returns <c>true</c> if the principal has a claim of <paramref name="claimType"/>
    /// with the specified <paramref name="value"/> (ordinal, case-insensitive comparison).
    /// </summary>
    public static bool HasClaim(this ClaimsPrincipal principal, string claimType, string value)
    {
        ArgumentNullException.ThrowIfNull(principal);
        ArgumentException.ThrowIfNullOrWhiteSpace(claimType);
        return principal.HasClaim(claimType, value);
    }
}
