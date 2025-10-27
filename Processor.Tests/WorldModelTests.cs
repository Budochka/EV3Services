using Xunit;
using Processor;

namespace Processor.Tests;

public class WorldModelTests
{
    [Fact]
    public void WorldModel_InitialDistance_ShouldBeZero()
    {
    // Arrange & Act
        var worldModel = new WorldModel();

     // Assert
        Assert.Equal(0, worldModel.Distanse);
    }

  [Fact]
 public void WorldModel_SetDistance_ShouldUpdateValue()
    {
     // Arrange
   var worldModel = new WorldModel();
        float expectedDistance = 42.5f;

     // Act
        worldModel.Distanse = expectedDistance;

   // Assert
  Assert.Equal(expectedDistance, worldModel.Distanse);
    }

    [Fact]
    public void WorldModel_Reset_ShouldSetDistanceToZero()
    {
        // Arrange
        var worldModel = new WorldModel();
        worldModel.Distanse = 100.0f;

        // Act
        worldModel.Reset();

        // Assert
        Assert.Equal(0, worldModel.Distanse);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(10.5f)]
    [InlineData(-5.3f)]
    [InlineData(1000.0f)]
    public void WorldModel_Distance_ShouldAcceptVariousValues(float distance)
    {
    // Arrange
        var worldModel = new WorldModel();

        // Act
        worldModel.Distanse = distance;

        // Assert
        Assert.Equal(distance, worldModel.Distanse);
    }
}
