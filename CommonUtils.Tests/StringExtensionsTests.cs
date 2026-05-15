using CommonUtils.Extensions;

namespace CommonUtils.Tests;

public class StringExtensionsTests
{
    // ── Normalize ─────────────────────────────────────────────────────────────

    // NOTE: StringExtensions.Normalize must be called as a static method because
    // string.Normalize() (Unicode normalization) is an instance method that takes
    // precedence over extension methods with the same name.

    [Fact]
    public void Normalize_WithNull_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => StringExtensions.Normalize(null!));
    }

    [Fact]
    public void Normalize_TrimsLeadingAndTrailingWhitespace()
    {
        Assert.Equal("hello", StringExtensions.Normalize("  hello  "));
    }

    [Fact]
    public void Normalize_CollapsesMultipleSpaces()
    {
        Assert.Equal("hello world", StringExtensions.Normalize("hello   world"));
    }

    [Fact]
    public void Normalize_CollapsesMixedWhitespaceCharacters()
    {
        Assert.Equal("hello world foo", StringExtensions.Normalize("  hello\t world  \n foo  "));
    }

    [Fact]
    public void Normalize_SingleWord_ReturnsUnchanged()
    {
        Assert.Equal("hello", StringExtensions.Normalize("hello"));
    }

    [Fact]
    public void Normalize_AlreadyNormalized_ReturnsUnchanged()
    {
        Assert.Equal("hello world", StringExtensions.Normalize("hello world"));
    }

    // ── Truncate ─────────────────────────────────────────────────────────────

    [Fact]
    public void Truncate_WithNull_ThrowsArgumentNullException()
    {
        string? value = null;
        Assert.Throws<ArgumentNullException>(() => value!.Truncate(5));
    }

    [Fact]
    public void Truncate_WithZeroMaxLength_ThrowsArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => "hello".Truncate(0));
    }

    [Fact]
    public void Truncate_WithNegativeMaxLength_ThrowsArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => "hello".Truncate(-1));
    }

    [Fact]
    public void Truncate_StringShorterThanMax_ReturnsOriginal()
    {
        Assert.Equal("hi", "hi".Truncate(10));
    }

    [Fact]
    public void Truncate_StringExactlyMax_ReturnsOriginal()
    {
        Assert.Equal("hello", "hello".Truncate(5));
    }

    [Fact]
    public void Truncate_StringLongerThanMax_ReturnsTruncated()
    {
        Assert.Equal("hello", "hello world".Truncate(5));
    }

    [Fact]
    public void Truncate_MaxLengthOne_ReturnsSingleChar()
    {
        Assert.Equal("h", "hello".Truncate(1));
    }

    // ── NullIfEmpty ──────────────────────────────────────────────────────────

    [Fact]
    public void NullIfEmpty_WithNull_ReturnsNull()
    {
        Assert.Null(((string?)null).NullIfEmpty());
    }

    [Fact]
    public void NullIfEmpty_WithEmptyString_ReturnsNull()
    {
        Assert.Null("".NullIfEmpty());
    }

    [Fact]
    public void NullIfEmpty_WithWhitespaceOnly_ReturnsNull()
    {
        Assert.Null("   ".NullIfEmpty());
    }

    [Fact]
    public void NullIfEmpty_WithWhitespaceAndTabs_ReturnsNull()
    {
        Assert.Null("  \t  ".NullIfEmpty());
    }

    [Fact]
    public void NullIfEmpty_WithValidString_ReturnsOriginalValue()
    {
        Assert.Equal("hello", "hello".NullIfEmpty());
    }

    [Fact]
    public void NullIfEmpty_WithValueContainingSpaces_ReturnsOriginalValue()
    {
        Assert.Equal("  hello  ", "  hello  ".NullIfEmpty());
    }

    // ── ToSnakeCase ──────────────────────────────────────────────────────────

    [Fact]
    public void ToSnakeCase_WithNull_ReturnsNull()
    {
        string? value = null;
        string result = value!.ToSnakeCase();
        Assert.Null(result);
    }

    [Fact]
    public void ToSnakeCase_WithEmptyString_ReturnsEmpty()
    {
        Assert.Equal("", "".ToSnakeCase());
    }

    [Fact]
    public void ToSnakeCase_PascalCase_ConvertsToSnakeCase()
    {
        Assert.Equal("order_id", "OrderId".ToSnakeCase());
    }

    [Fact]
    public void ToSnakeCase_CamelCase_ConvertsToSnakeCase()
    {
        Assert.Equal("order_id", "orderId".ToSnakeCase());
    }

    [Fact]
    public void ToSnakeCase_MultipleWords_ConvertsAllTransitions()
    {
        Assert.Equal("user_first_name", "UserFirstName".ToSnakeCase());
    }

    [Fact]
    public void ToSnakeCase_AlreadyLowercase_ReturnsUnchanged()
    {
        Assert.Equal("hello", "hello".ToSnakeCase());
    }

    [Fact]
    public void ToSnakeCase_WithDigitBeforeUppercase_InsertsUnderscore()
    {
        // Digit counts as [a-z0-9] so triggers a break before the next uppercase.
        Assert.Equal("version2_value", "Version2Value".ToSnakeCase());
    }

    [Fact]
    public void ToSnakeCase_AllUppercaseAcronymPrefix_OnlyBreaksAtLowercaseToUppercaseTransition()
    {
        // HTTP has no lowercase → uppercase transition internally;
        // the break only happens at r→E (Server→Error).
        Assert.Equal("httpserver_error", "HTTPServerError".ToSnakeCase());
    }

    [Fact]
    public void ToSnakeCase_TrailingAcronym_ConvertsCorrectly()
    {
        // r (from Order) → I (from ID) is a lowercase → uppercase transition.
        Assert.Equal("order_id", "OrderID".ToSnakeCase());
    }
}
