using Xunit;

namespace Videoteket.Tests;

public class LateFeeServiceTests
{
    [Fact]
    public void CalculateLateFee_ThreeDaysLate_ReturnsCorrectFee()
    {
        // Arrange
        int lateDays = 3;
        decimal rentalPrice = 20m;

        // Act
        decimal result = LateFeeService.CalculateLateFee(lateDays, rentalPrice);

        // Assert
        Assert.Equal(60m, result);
    }
}