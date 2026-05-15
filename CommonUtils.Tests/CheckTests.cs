using CommonUtils.Checks;

namespace CommonUtils.Tests;

public class CheckTests
{
    // ── NotNull ──────────────────────────────────────────────────────────────

    [Fact]
    public void NotNull_WithNonNullValue_ReturnsValue()
    {
        var obj = new object();
        Assert.Same(obj, Check.NotNull(obj, "param"));
    }

    [Fact]
    public void NotNull_WithNull_ThrowsArgumentNullException()
    {
        var ex = Assert.Throws<ArgumentNullException>(() => Check.NotNull<object>(null, "param"));
        Assert.Equal("param", ex.ParamName);
    }

    // ── NotEmpty(string) ─────────────────────────────────────────────────────

    [Fact]
    public void NotEmpty_String_WithValidValue_ReturnsTrimmedValue()
    {
        Assert.Equal("hello", Check.NotEmpty("  hello  ", "param"));
    }

    [Fact]
    public void NotEmpty_String_WithNull_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => Check.NotEmpty(null, "param"));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("\t")]
    public void NotEmpty_String_WithEmptyOrWhitespace_ThrowsArgumentException(string value)
    {
        Assert.Throws<ArgumentException>(() => Check.NotEmpty(value, "param"));
    }

    // ── MaxLength ────────────────────────────────────────────────────────────

    [Fact]
    public void MaxLength_WithinLimit_ReturnsOriginalValue()
    {
        Assert.Equal("hi", Check.MaxLength("hi", 10, "param"));
    }

    [Fact]
    public void MaxLength_AtExactLimit_ReturnsValue()
    {
        Assert.Equal("hello", Check.MaxLength("hello", 5, "param"));
    }

    [Fact]
    public void MaxLength_ExceedsLimit_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => Check.MaxLength("toolong", 3, "param"));
    }

    [Fact]
    public void MaxLength_EmptyString_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => Check.MaxLength("", 10, "param"));
    }

    [Fact]
    public void MaxLength_ValidatesTrimedLengthButReturnsOriginal()
    {
        // Trimmed "hi" is 2 chars — passes the 2-char max; original (padded) is returned.
        Assert.Equal("  hi  ", Check.MaxLength("  hi  ", 2, "param"));
    }

    // ── MinLength ────────────────────────────────────────────────────────────

    [Fact]
    public void MinLength_AboveMinimum_ReturnsOriginalValue()
    {
        Assert.Equal("hello world", Check.MinLength("hello world", 3, "param"));
    }

    [Fact]
    public void MinLength_AtExactMinimum_ReturnsValue()
    {
        Assert.Equal("hi", Check.MinLength("hi", 2, "param"));
    }

    [Fact]
    public void MinLength_BelowMinimum_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => Check.MinLength("ab", 5, "param"));
    }

    [Fact]
    public void MinLength_ValidatesTrimedLengthButReturnsOriginal()
    {
        // Trimmed "hello" is 5 chars — passes the 3-char min; original (padded) is returned.
        Assert.Equal("  hello  ", Check.MinLength("  hello  ", 3, "param"));
    }

    // ── Length ───────────────────────────────────────────────────────────────

    [Fact]
    public void Length_WithinRange_ReturnsValue()
    {
        Assert.Equal("hello", Check.Length("hello", 3, 10, "param"));
    }

    [Fact]
    public void Length_AtMinBoundary_ReturnsValue()
    {
        Assert.Equal("hi", Check.Length("hi", 2, 10, "param"));
    }

    [Fact]
    public void Length_AtMaxBoundary_ReturnsValue()
    {
        Assert.Equal("hello", Check.Length("hello", 2, 5, "param"));
    }

    [Fact]
    public void Length_BelowMin_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => Check.Length("ab", 3, 10, "param"));
    }

    [Fact]
    public void Length_AboveMax_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => Check.Length("hello world", 3, 5, "param"));
    }

    // ── Positive(int) ────────────────────────────────────────────────────────

    [Fact]
    public void Positive_Int_WithPositiveValue_ReturnsValue()
    {
        Assert.Equal(5, Check.Positive(5, "param"));
    }

    [Fact]
    public void Positive_Int_WithOne_ReturnsOne()
    {
        Assert.Equal(1, Check.Positive(1, "param"));
    }

    [Fact]
    public void Positive_Int_WithZero_ThrowsArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => Check.Positive(0, "param"));
    }

    [Fact]
    public void Positive_Int_WithNegative_ThrowsArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => Check.Positive(-1, "param"));
    }

    // ── NotNegative(int) ─────────────────────────────────────────────────────

    [Fact]
    public void NotNegative_Int_WithZero_ReturnsZero()
    {
        Assert.Equal(0, Check.NotNegative(0, "param"));
    }

    [Fact]
    public void NotNegative_Int_WithPositive_ReturnsValue()
    {
        Assert.Equal(10, Check.NotNegative(10, "param"));
    }

    [Fact]
    public void NotNegative_Int_WithNegative_ThrowsArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => Check.NotNegative(-1, "param"));
    }

    // ── InRange(int) ─────────────────────────────────────────────────────────

    [Fact]
    public void InRange_Int_WithinRange_ReturnsValue()
    {
        Assert.Equal(5, Check.InRange(5, 1, 10, "param"));
    }

    [Fact]
    public void InRange_Int_AtMinBoundary_ReturnsValue()
    {
        Assert.Equal(1, Check.InRange(1, 1, 10, "param"));
    }

    [Fact]
    public void InRange_Int_AtMaxBoundary_ReturnsValue()
    {
        Assert.Equal(10, Check.InRange(10, 1, 10, "param"));
    }

    [Fact]
    public void InRange_Int_BelowMin_ThrowsArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => Check.InRange(0, 1, 10, "param"));
    }

    [Fact]
    public void InRange_Int_AboveMax_ThrowsArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => Check.InRange(11, 1, 10, "param"));
    }

    // ── Positive(long) ───────────────────────────────────────────────────────

    [Fact]
    public void Positive_Long_WithPositiveValue_ReturnsValue()
    {
        Assert.Equal(100L, Check.Positive(100L, "param"));
    }

    [Fact]
    public void Positive_Long_WithOne_ReturnsOne()
    {
        Assert.Equal(1L, Check.Positive(1L, "param"));
    }

    [Fact]
    public void Positive_Long_WithZero_ThrowsArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => Check.Positive(0L, "param"));
    }

    [Fact]
    public void Positive_Long_WithNegative_ThrowsArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => Check.Positive(-1L, "param"));
    }

    // ── NotNegative(long) ────────────────────────────────────────────────────

    [Fact]
    public void NotNegative_Long_WithZero_ReturnsZero()
    {
        Assert.Equal(0L, Check.NotNegative(0L, "param"));
    }

    [Fact]
    public void NotNegative_Long_WithPositive_ReturnsValue()
    {
        Assert.Equal(99L, Check.NotNegative(99L, "param"));
    }

    [Fact]
    public void NotNegative_Long_WithNegative_ThrowsArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => Check.NotNegative(-1L, "param"));
    }

    // ── Positive(decimal) ────────────────────────────────────────────────────

    [Fact]
    public void Positive_Decimal_WithPositiveValue_ReturnsValue()
    {
        Assert.Equal(9.99m, Check.Positive(9.99m, "param"));
    }

    [Fact]
    public void Positive_Decimal_WithSmallestPositive_ReturnsValue()
    {
        Assert.Equal(0.01m, Check.Positive(0.01m, "param"));
    }

    [Fact]
    public void Positive_Decimal_WithZero_ThrowsArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => Check.Positive(0m, "param"));
    }

    [Fact]
    public void Positive_Decimal_WithNegative_ThrowsArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => Check.Positive(-0.01m, "param"));
    }

    // ── NotNegative(decimal) ─────────────────────────────────────────────────

    [Fact]
    public void NotNegative_Decimal_WithZero_ReturnsZero()
    {
        Assert.Equal(0m, Check.NotNegative(0m, "param"));
    }

    [Fact]
    public void NotNegative_Decimal_WithPositive_ReturnsValue()
    {
        Assert.Equal(1.5m, Check.NotNegative(1.5m, "param"));
    }

    [Fact]
    public void NotNegative_Decimal_WithNegative_ThrowsArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => Check.NotNegative(-0.01m, "param"));
    }

    // ── InRange(decimal) ─────────────────────────────────────────────────────

    [Fact]
    public void InRange_Decimal_WithinRange_ReturnsValue()
    {
        Assert.Equal(5.5m, Check.InRange(5.5m, 1m, 10m, "param"));
    }

    [Fact]
    public void InRange_Decimal_AtMinBoundary_ReturnsValue()
    {
        Assert.Equal(1m, Check.InRange(1m, 1m, 10m, "param"));
    }

    [Fact]
    public void InRange_Decimal_AtMaxBoundary_ReturnsValue()
    {
        Assert.Equal(10m, Check.InRange(10m, 1m, 10m, "param"));
    }

    [Fact]
    public void InRange_Decimal_BelowMin_ThrowsArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => Check.InRange(0.99m, 1m, 10m, "param"));
    }

    [Fact]
    public void InRange_Decimal_AboveMax_ThrowsArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => Check.InRange(10.01m, 1m, 10m, "param"));
    }

    // ── Positive(double) ─────────────────────────────────────────────────────

    [Fact]
    public void Positive_Double_WithPositiveValue_ReturnsValue()
    {
        Assert.Equal(3.14d, Check.Positive(3.14d, "param"));
    }

    [Fact]
    public void Positive_Double_WithZero_ThrowsArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => Check.Positive(0d, "param"));
    }

    [Fact]
    public void Positive_Double_WithNegative_ThrowsArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => Check.Positive(-1d, "param"));
    }

    // ── NotNegative(double) ──────────────────────────────────────────────────

    [Fact]
    public void NotNegative_Double_WithZero_ReturnsZero()
    {
        Assert.Equal(0d, Check.NotNegative(0d, "param"));
    }

    [Fact]
    public void NotNegative_Double_WithPositive_ReturnsValue()
    {
        Assert.Equal(2.5d, Check.NotNegative(2.5d, "param"));
    }

    [Fact]
    public void NotNegative_Double_WithNegative_ThrowsArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => Check.NotNegative(-0.1d, "param"));
    }

    // ── NotEmpty(IEnumerable<T>) ─────────────────────────────────────────────

    [Fact]
    public void NotEmpty_Collection_WithNonEmptyList_ReturnsCollection()
    {
        var list = new List<int> { 1, 2, 3 };
        var result = Check.NotEmpty(list, "param");
        Assert.Same(list, result);
    }

    [Fact]
    public void NotEmpty_Collection_WithNull_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => Check.NotEmpty<int>(null, "param"));
    }

    [Fact]
    public void NotEmpty_Collection_WithEmptyList_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => Check.NotEmpty(new List<int>(), "param"));
    }

    // ── MaxCount ─────────────────────────────────────────────────────────────

    [Fact]
    public void MaxCount_WithinLimit_ReturnsCollection()
    {
        var list = new List<int> { 1, 2 };
        Assert.Same(list, Check.MaxCount(list, 5, "param"));
    }

    [Fact]
    public void MaxCount_AtExactLimit_ReturnsCollection()
    {
        var list = new List<int> { 1, 2, 3 };
        Assert.Same(list, Check.MaxCount(list, 3, "param"));
    }

    [Fact]
    public void MaxCount_ExceedsLimit_ThrowsArgumentException()
    {
        var list = new List<int> { 1, 2, 3, 4 };
        Assert.Throws<ArgumentException>(() => Check.MaxCount(list, 3, "param"));
    }

    [Fact]
    public void MaxCount_WithNull_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => Check.MaxCount<int>(null!, 3, "param"));
    }

    // ── MinCount ─────────────────────────────────────────────────────────────

    [Fact]
    public void MinCount_AboveMinimum_ReturnsCollection()
    {
        var list = new List<int> { 1, 2, 3 };
        Assert.Same(list, Check.MinCount(list, 2, "param"));
    }

    [Fact]
    public void MinCount_AtExactMinimum_ReturnsCollection()
    {
        var list = new List<int> { 1, 2 };
        Assert.Same(list, Check.MinCount(list, 2, "param"));
    }

    [Fact]
    public void MinCount_BelowMinimum_ThrowsArgumentException()
    {
        var list = new List<int> { 1 };
        Assert.Throws<ArgumentException>(() => Check.MinCount(list, 2, "param"));
    }

    [Fact]
    public void MinCount_WithNull_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => Check.MinCount<int>(null!, 2, "param"));
    }

    // ── NotEmpty(Guid) ───────────────────────────────────────────────────────

    [Fact]
    public void NotEmpty_Guid_WithValidGuid_ReturnsGuid()
    {
        var guid = Guid.NewGuid();
        Assert.Equal(guid, Check.NotEmpty(guid, "param"));
    }

    [Fact]
    public void NotEmpty_Guid_WithEmptyGuid_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => Check.NotEmpty(Guid.Empty, "param"));
    }

    // ── Defined(Enum) ────────────────────────────────────────────────────────

    private enum Color { Red = 1, Green = 2, Blue = 3 }

    [Fact]
    public void Defined_WithDefinedValue_ReturnsValue()
    {
        Assert.Equal(Color.Red, Check.Defined(Color.Red, "param"));
    }

    [Fact]
    public void Defined_WithUndefinedValue_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => Check.Defined((Color)99, "param"));
    }

    // ── NotDefault(DateTime) ─────────────────────────────────────────────────

    [Fact]
    public void NotDefault_DateTime_WithValidDate_ReturnsDate()
    {
        var dt = new DateTime(2024, 6, 1, 0, 0, 0, DateTimeKind.Utc);
        Assert.Equal(dt, Check.NotDefault(dt, "param"));
    }

    [Fact]
    public void NotDefault_DateTime_WithDefault_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => Check.NotDefault(default(DateTime), "param"));
    }

    // ── NotDefault(DateTimeOffset) ───────────────────────────────────────────

    [Fact]
    public void NotDefault_DateTimeOffset_WithValidDate_ReturnsDate()
    {
        var dto = new DateTimeOffset(2024, 6, 1, 0, 0, 0, TimeSpan.Zero);
        Assert.Equal(dto, Check.NotDefault(dto, "param"));
    }

    [Fact]
    public void NotDefault_DateTimeOffset_WithDefault_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => Check.NotDefault(default(DateTimeOffset), "param"));
    }

    // ── NotInPast(DateTime) ──────────────────────────────────────────────────

    [Fact]
    public void NotInPast_DateTime_WithFutureDate_ReturnsDate()
    {
        var future = DateTime.UtcNow.AddDays(1);
        Assert.Equal(future, Check.NotInPast(future, "param"));
    }

    [Fact]
    public void NotInPast_DateTime_WithPastDate_ThrowsArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => Check.NotInPast(DateTime.UtcNow.AddDays(-1), "param"));
    }

    [Fact]
    public void NotInPast_DateTime_WithDefault_ThrowsArgumentException()
    {
        // NotDefault short-circuit fires before the past check.
        Assert.Throws<ArgumentException>(() => Check.NotInPast(default(DateTime), "param"));
    }

    // ── NotInFuture(DateTime) ────────────────────────────────────────────────

    [Fact]
    public void NotInFuture_DateTime_WithPastDate_ReturnsDate()
    {
        var past = DateTime.UtcNow.AddDays(-1);
        Assert.Equal(past, Check.NotInFuture(past, "param"));
    }

    [Fact]
    public void NotInFuture_DateTime_WithFutureDate_ThrowsArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => Check.NotInFuture(DateTime.UtcNow.AddDays(1), "param"));
    }

    [Fact]
    public void NotInFuture_DateTime_WithDefault_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => Check.NotInFuture(default(DateTime), "param"));
    }

    // ── NotInPast(DateTimeOffset) ────────────────────────────────────────────

    [Fact]
    public void NotInPast_DateTimeOffset_WithFutureDate_ReturnsDate()
    {
        var future = DateTimeOffset.UtcNow.AddDays(1);
        Assert.Equal(future, Check.NotInPast(future, "param"));
    }

    [Fact]
    public void NotInPast_DateTimeOffset_WithPastDate_ThrowsArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => Check.NotInPast(DateTimeOffset.UtcNow.AddDays(-1), "param"));
    }

    [Fact]
    public void NotInPast_DateTimeOffset_WithDefault_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => Check.NotInPast(default(DateTimeOffset), "param"));
    }

    // ── NotInFuture(DateTimeOffset) ──────────────────────────────────────────

    [Fact]
    public void NotInFuture_DateTimeOffset_WithPastDate_ReturnsDate()
    {
        var past = DateTimeOffset.UtcNow.AddDays(-1);
        Assert.Equal(past, Check.NotInFuture(past, "param"));
    }

    [Fact]
    public void NotInFuture_DateTimeOffset_WithFutureDate_ThrowsArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => Check.NotInFuture(DateTimeOffset.UtcNow.AddDays(1), "param"));
    }

    [Fact]
    public void NotInFuture_DateTimeOffset_WithDefault_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => Check.NotInFuture(default(DateTimeOffset), "param"));
    }
}
