namespace Swegon.Recruitment.CodeTest.Backend.Api.Contracts.Enums;

public enum ErrorCode
{
    Unknown = 0,
    ValidationError = 1000,
    NotFound = 2000,
    Unauthorized = 3000,
    InternalError = 5000,
    ExternalServiceError = 6000,
    CalculationError = 7000,
}
