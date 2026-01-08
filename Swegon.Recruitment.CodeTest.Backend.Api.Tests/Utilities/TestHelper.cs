using FluentAssertions;
using Swegon.Recruitment.CodeTest.Backend.Api.Models;
using Swegon.Recruitment.CodeTest.Backend.Api.Contracts.Enums;

namespace Swegon.Recruitment.CodeTest.Backend.Api.Tests.Utilities;

/// <summary>
/// Common test utilities for assertions and comparisons
/// </summary>
public static class TestHelper
{
    /// <summary>
    /// Asserts that two products are equal
    /// </summary>
    public static void AssertProductsEqual(Product expected, Product actual)
    {
        actual.Should().NotBeNull();
        actual.Id.Should().Be(expected.Id);
        actual.Name.Should().Be(expected.Name);
        actual.Description.Should().Be(expected.Description);
        actual.Type.Should().Be(expected.Type);
        actual.SKU.Should().Be(expected.SKU);
        actual.Price.Should().Be(expected.Price);
        actual.IsActive.Should().Be(expected.IsActive);
    }

    /// <summary>
    /// Asserts that two calculations are equal
    /// </summary>
    public static void AssertCalculationsEqual(Calculation expected, Calculation actual)
    {
        actual.Should().NotBeNull();
        actual.Id.Should().Be(expected.Id);
        actual.ProductId.Should().Be(expected.ProductId);
        actual.Status.Should().Be(expected.Status);
        actual.Total.Should().Be(expected.Total);
        actual.Subtotal.Should().Be(expected.Subtotal);
        actual.DiscountAmount.Should().Be(expected.DiscountAmount);
        actual.TaxAmount.Should().Be(expected.TaxAmount);
        actual.Currency.Should().Be(expected.Currency);
    }

    /// <summary>
    /// Asserts that a collection contains items matching predicate
    /// </summary>
    public static void AssertContains<T>(IEnumerable<T> collection, Func<T, bool> predicate, string message = "")
    {
        collection.Should().NotBeNull();
        collection.Should().Contain(predicate, message);
    }

    /// <summary>
    /// Asserts that a value is within a specified range
    /// </summary>
    public static void AssertInRange(decimal actual, decimal min, decimal max)
    {
        actual.Should().BeGreaterOrEqualTo(min);
        actual.Should().BeLessOrEqualTo(max);
    }

    /// <summary>
    /// Asserts that a date is recent (within last 5 seconds)
    /// </summary>
    public static void AssertRecentDate(DateTime date)
    {
        var now = DateTime.UtcNow;
        date.Should().BeAfter(now.AddSeconds(-5));
        date.Should().BeBefore(now.AddSeconds(1));
    }

    /// <summary>
    /// Asserts that a calculation result has valid values
    /// </summary>
    public static void AssertValidCalculationResult(CalculationResult result)
    {
        result.Should().NotBeNull();
        result.Status.Should().Be(CalculationStatus.Completed);
        result.Total.Should().BeGreaterOrEqualTo(0);
        result.Subtotal.Should().BeGreaterOrEqualTo(0);
        result.BasePrice.Should().BeGreaterThan(0);
        result.Quantity.Should().BeGreaterThan(0);
    }

    /// <summary>
    /// Creates a valid product ID
    /// </summary>
    public static Guid CreateValidId() => Guid.NewGuid();

    /// <summary>
    /// Creates a collection of IDs
    /// </summary>
    public static List<Guid> CreateIds(int count)
    {
        return Enumerable.Range(0, count).Select(_ => Guid.NewGuid()).ToList();
    }

    /// <summary>
    /// Asserts that a string is not null or empty
    /// </summary>
    public static void AssertNotNullOrEmpty(string? value, string paramName)
    {
        value.Should().NotBeNullOrEmpty(paramName);
    }

    /// <summary>
    /// Asserts that a collection is not null or empty
    /// </summary>
    public static void AssertNotNullOrEmpty<T>(IEnumerable<T>? collection, string paramName)
    {
        collection.Should().NotBeNullOrEmpty(paramName);
    }

    /// <summary>
    /// Compares two decimal values with tolerance
    /// </summary>
    public static void AssertDecimalEqual(decimal expected, decimal actual, decimal tolerance = 0.01m)
    {
        Math.Abs(expected - actual).Should().BeLessOrEqualTo(tolerance);
    }

    /// <summary>
    /// Asserts that metadata contains a specific key
    /// </summary>
    public static void AssertMetadataContainsKey(Dictionary<string, object> metadata, string key)
    {
        metadata.Should().ContainKey(key);
    }

    /// <summary>
    /// Asserts that a product has valid default values
    /// </summary>
    public static void AssertValidProduct(Product product)
    {
        product.Should().NotBeNull();
        product.Id.Should().NotBe(Guid.Empty);
        product.Name.Should().NotBeNullOrEmpty();
        product.Price.Should().BeGreaterOrEqualTo(0);
    }

    /// <summary>
    /// Asserts that an exception was thrown with specific message
    /// </summary>
    public static void AssertExceptionWithMessage<TException>(Action action, string expectedMessage) 
        where TException : Exception
    {
        var exception = Assert.Throws<TException>(action);
        exception.Message.Should().Contain(expectedMessage);
    }

    /// <summary>
    /// Asserts that an async exception was thrown
    /// </summary>
    public static async Task AssertThrowsAsync<TException>(Func<Task> action) 
        where TException : Exception
    {
        await Assert.ThrowsAsync<TException>(action);
    }

    /// <summary>
    /// Creates a random string of specified length
    /// </summary>
    public static string RandomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var random = new Random();
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }

    /// <summary>
    /// Creates a random decimal in range
    /// </summary>
    public static decimal RandomDecimal(decimal min, decimal max)
    {
        var random = new Random();
        var range = max - min;
        return min + (decimal)random.NextDouble() * range;
    }

    /// <summary>
    /// Creates a random integer in range
    /// </summary>
    public static int RandomInt(int min, int max)
    {
        var random = new Random();
        return random.Next(min, max);
    }

    /// <summary>
    /// Asserts that two lists have the same items (order doesn't matter)
    /// </summary>
    public static void AssertListsEquivalent<T>(IEnumerable<T> expected, IEnumerable<T> actual)
    {
        actual.Should().BeEquivalentTo(expected);
    }

    /// <summary>
    /// Asserts that a result is successful
    /// </summary>
    public static void AssertSuccessResult(Result result)
    {
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    /// <summary>
    /// Asserts that a result failed with specific error
    /// </summary>
    public static void AssertFailureResult(Result result, string errorMessage)
    {
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Contains(errorMessage));
    }

    /// <summary>
    /// Creates test parameters dictionary
    /// </summary>
    public static Dictionary<string, object> CreateTestParameters(params (string key, object value)[] parameters)
    {
        var dict = new Dictionary<string, object>();
        foreach (var (key, value) in parameters)
        {
            dict[key] = value;
        }
        return dict;
    }

    /// <summary>
    /// Waits for a condition to be true
    /// </summary>
    public static async Task WaitForCondition(Func<bool> condition, int timeoutMs = 5000)
    {
        var startTime = DateTime.UtcNow;
        while (!condition() && (DateTime.UtcNow - startTime).TotalMilliseconds < timeoutMs)
        {
            await Task.Delay(100);
        }
        condition().Should().BeTrue("Condition was not met within timeout");
    }
}
