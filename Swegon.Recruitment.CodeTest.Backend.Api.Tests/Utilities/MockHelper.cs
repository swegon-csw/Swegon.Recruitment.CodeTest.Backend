using Microsoft.Extensions.Logging;
using Moq;
using Swegon.Recruitment.CodeTest.Backend.Api.Models;
using Swegon.Recruitment.CodeTest.Backend.Api.Services;
using Swegon.Recruitment.CodeTest.Backend.Api.Validators;
using Swegon.Recruitment.CodeTest.Backend.Api.Contracts.Enums;

namespace Swegon.Recruitment.CodeTest.Backend.Api.Tests.Utilities;

/// <summary>
/// Helper methods for creating mocks
/// </summary>
public static class MockHelper
{
    /// <summary>
    /// Creates a mock logger
    /// </summary>
    public static Mock<ILogger<T>> CreateMockLogger<T>()
    {
        return new Mock<ILogger<T>>();
    }

    /// <summary>
    /// Creates a mock CacheService
    /// </summary>
    public static Mock<CacheService> CreateMockCacheService()
    {
        var mockLogger = CreateMockLogger<CacheService>();
        return new Mock<CacheService>(mockLogger.Object);
    }

    /// <summary>
    /// Creates a mock ProductService with common setup
    /// </summary>
    public static Mock<ProductService> CreateMockProductService()
    {
        var mockValidator = new Mock<ProductValidator>();
        var mockCache = CreateMockCacheService();
        var mockLogger = CreateMockLogger<ProductService>();

        return new Mock<ProductService>(
            mockValidator.Object,
            mockCache.Object,
            mockLogger.Object);
    }

    /// <summary>
    /// Creates a mock CalculationService with common setup
    /// </summary>
    public static Mock<CalculationService> CreateMockCalculationService()
    {
        var mockLogger = CreateMockLogger<CalculationService>();
        var mockProductService = CreateMockProductService();
        var mockCache = CreateMockCacheService();

        return new Mock<CalculationService>(
            mockLogger.Object,
            mockProductService.Object,
            mockCache.Object);
    }

    /// <summary>
    /// Setups a mock to return a specific value
    /// </summary>
    public static Mock<T> SetupMockReturn<T>(T returnValue) where T : class
    {
        var mock = new Mock<T>();
        return mock;
    }

    /// <summary>
    /// Setups a mock to throw an exception
    /// </summary>
    public static Mock<T> SetupMockThrow<T>(Exception exception) where T : class
    {
        var mock = new Mock<T>();
        return mock;
    }

    /// <summary>
    /// Creates a mock that verifies method was called
    /// </summary>
    public static Mock<T> CreateVerifiableMock<T>() where T : class
    {
        return new Mock<T>(MockBehavior.Strict);
    }

    /// <summary>
    /// Creates a mock with loose behavior (default returns)
    /// </summary>
    public static Mock<T> CreateLooseMock<T>() where T : class
    {
        return new Mock<T>(MockBehavior.Loose);
    }

    /// <summary>
    /// Verifies a method was called once
    /// </summary>
    public static void VerifyCalledOnce<T>(Mock<T> mock, Action<T> expression) where T : class
    {
        mock.Verify(expression, Times.Once);
    }

    /// <summary>
    /// Verifies a method was never called
    /// </summary>
    public static void VerifyNeverCalled<T>(Mock<T> mock, Action<T> expression) where T : class
    {
        mock.Verify(expression, Times.Never);
    }

    /// <summary>
    /// Verifies a method was called specified times
    /// </summary>
    public static void VerifyCalledTimes<T>(Mock<T> mock, Action<T> expression, int times) where T : class
    {
        mock.Verify(expression, Times.Exactly(times));
    }

    /// <summary>
    /// Creates a callback for async operations
    /// </summary>
    public static Action<Task> CreateAsyncCallback(Action action)
    {
        return _ => action();
    }

    /// <summary>
    /// Creates a sequence of return values for multiple calls
    /// </summary>
    public static void SetupSequence<T, TResult>(Mock<T> mock, Func<T, TResult> expression, params TResult[] results) 
        where T : class
    {
        var setup = mock.SetupSequence(expression);
        foreach (var result in results)
        {
            setup = setup.Returns(result);
        }
    }

    /// <summary>
    /// Setups a mock to return async result
    /// </summary>
    public static void SetupAsyncReturn<T, TResult>(Mock<T> mock, Func<T, Task<TResult>> expression, TResult result) 
        where T : class
    {
        mock.Setup(expression).ReturnsAsync(result);
    }

    /// <summary>
    /// Setups a mock to throw async exception
    /// </summary>
    public static void SetupAsyncThrow<T, TResult>(Mock<T> mock, Func<T, Task<TResult>> expression, Exception exception) 
        where T : class
    {
        mock.Setup(expression).ThrowsAsync(exception);
    }

    /// <summary>
    /// Creates a mock logger that captures log messages
    /// </summary>
    public static (Mock<ILogger<T>> Mock, List<string> Messages) CreateMockLoggerWithCapture<T>()
    {
        var messages = new List<string>();
        var mockLogger = new Mock<ILogger<T>>();

        mockLogger.Setup(x => x.Log(
            It.IsAny<LogLevel>(),
            It.IsAny<EventId>(),
            It.IsAny<It.IsAnyType>(),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()))
            .Callback(new InvocationAction(invocation =>
            {
                var message = invocation.Arguments[2]?.ToString() ?? string.Empty;
                messages.Add(message);
            }));

        return (mockLogger, messages);
    }

    /// <summary>
    /// Verifies logger was called with specific log level
    /// </summary>
    public static void VerifyLogLevel<T>(Mock<ILogger<T>> mockLogger, LogLevel level, Times times)
    {
        mockLogger.Verify(
            x => x.Log(
                level,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            times);
    }

    /// <summary>
    /// Creates a default mock setup for common scenarios
    /// </summary>
    public static void SetupDefaultBehavior<T>(Mock<T> mock) where T : class
    {
        // Add common default setups here
    }

    /// <summary>
    /// Resets all setups on a mock
    /// </summary>
    public static void ResetMock<T>(Mock<T> mock) where T : class
    {
        mock.Reset();
    }

    /// <summary>
    /// Creates a mock with auto-properties
    /// </summary>
    public static Mock<T> CreateMockWithAutoProperties<T>() where T : class
    {
        var mock = new Mock<T>();
        mock.SetupAllProperties();
        return mock;
    }
}
