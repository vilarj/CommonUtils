using System.Security.Claims;
using CommonUtils.Auth;

namespace CommonUtils.Tests;

public class ClaimsPrincipalExtensionsTests
{
    private static ClaimsPrincipal MakePrincipal(params Claim[] claims)
    {
        var identity = new ClaimsIdentity(claims, "test");
        return new ClaimsPrincipal(identity);
    }

    // ── GetUserId ─────────────────────────────────────────────────────────────

    [Fact]
    public void GetUserId_WhenClaimPresent_ReturnsValue()
    {
        var principal = MakePrincipal(new Claim(ClaimTypes.NameIdentifier, "user-123"));
        Assert.Equal("user-123", principal.GetUserId());
    }

    [Fact]
    public void GetUserId_WhenClaimAbsent_ReturnsNull()
    {
        var principal = MakePrincipal();
        Assert.Null(principal.GetUserId());
    }

    [Fact]
    public void GetUserId_NullPrincipal_ThrowsArgumentNullException()
    {
        ClaimsPrincipal? principal = null;
        Assert.Throws<ArgumentNullException>(() => principal!.GetUserId());
    }

    // ── GetEmail ──────────────────────────────────────────────────────────────

    [Fact]
    public void GetEmail_WhenClaimPresent_ReturnsValue()
    {
        var principal = MakePrincipal(new Claim(ClaimTypes.Email, "test@example.com"));
        Assert.Equal("test@example.com", principal.GetEmail());
    }

    [Fact]
    public void GetEmail_WhenClaimAbsent_ReturnsNull()
    {
        Assert.Null(MakePrincipal().GetEmail());
    }

    // ── GetRole ───────────────────────────────────────────────────────────────

    [Fact]
    public void GetRole_WhenClaimPresent_ReturnsFirstRole()
    {
        var principal = MakePrincipal(
            new Claim(ClaimTypes.Role, "Admin"),
            new Claim(ClaimTypes.Role, "User"));
        Assert.Equal("Admin", principal.GetRole());
    }

    [Fact]
    public void GetRole_WhenClaimAbsent_ReturnsNull()
    {
        Assert.Null(MakePrincipal().GetRole());
    }

    // ── GetRoles ──────────────────────────────────────────────────────────────

    [Fact]
    public void GetRoles_ReturnsAllRoles()
    {
        var principal = MakePrincipal(
            new Claim(ClaimTypes.Role, "Admin"),
            new Claim(ClaimTypes.Role, "Manager"));
        var roles = principal.GetRoles().ToList();
        Assert.Equal(2, roles.Count);
        Assert.Contains("Admin", roles);
        Assert.Contains("Manager", roles);
    }

    [Fact]
    public void GetRoles_WhenNonePresent_ReturnsEmpty()
    {
        Assert.Empty(MakePrincipal().GetRoles());
    }

    // ── GetClaim ──────────────────────────────────────────────────────────────

    [Fact]
    public void GetClaim_WhenClaimPresent_ReturnsValue()
    {
        var principal = MakePrincipal(new Claim("custom-type", "custom-value"));
        Assert.Equal("custom-value", principal.GetClaim("custom-type"));
    }

    [Fact]
    public void GetClaim_WhenAbsent_ReturnsNull()
    {
        Assert.Null(MakePrincipal().GetClaim("missing"));
    }

    [Fact]
    public void GetClaim_NullPrincipal_ThrowsArgumentNullException()
    {
        ClaimsPrincipal? principal = null;
        Assert.Throws<ArgumentNullException>(() => principal!.GetClaim("type"));
    }

    [Fact]
    public void GetClaim_NullOrWhiteSpaceType_ThrowsArgumentException()
    {
        var principal = MakePrincipal();
        Assert.Throws<ArgumentException>(() => principal.GetClaim("   "));
    }

    // ── HasClaim ──────────────────────────────────────────────────────────────

    [Fact]
    public void HasClaim_WhenMatchingClaimPresent_ReturnsTrue()
    {
        var principal = MakePrincipal(new Claim("role", "Admin"));
        Assert.True(principal.HasClaim("role", "Admin"));
    }

    [Fact]
    public void HasClaim_WhenValueDoesNotMatch_ReturnsFalse()
    {
        var principal = MakePrincipal(new Claim("role", "User"));
        Assert.False(principal.HasClaim("role", "Admin"));
    }

    [Fact]
    public void HasClaim_WhenAbsent_ReturnsFalse()
    {
        Assert.False(MakePrincipal().HasClaim("role", "Admin"));
    }
}
