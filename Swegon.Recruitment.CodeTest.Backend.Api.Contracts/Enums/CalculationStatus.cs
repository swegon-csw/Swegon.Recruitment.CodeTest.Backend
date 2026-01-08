namespace Swegon.Recruitment.CodeTest.Backend.Api.Contracts.Enums;

/// <summary>
/// Status of a calculation operation
/// </summary>
public enum CalculationStatus
{
    /// <summary>
    /// Calculation is pending
    /// </summary>
    Pending = 0,
    
    /// <summary>
    /// Calculation is in progress
    /// </summary>
    InProgress = 1,
    
    /// <summary>
    /// Calculation completed successfully
    /// </summary>
    Completed = 2,
    
    /// <summary>
    /// Calculation failed
    /// </summary>
    Failed = 3,
    
    /// <summary>
    /// Calculation was cancelled
    /// </summary>
    Cancelled = 4
}
