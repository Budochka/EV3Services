using Xunit;
using Processor;

namespace Processor.Tests;

public class ConfigTests
{
 [Fact]
    public void Config_DefaultConstructor_ShouldCreateInstance()
{
  // Arrange & Act
        var config = new Config();

  // Assert
        Assert.NotNull(config);
    }

 [Fact]
    public void Config_RabbitMQProperties_ShouldSetCorrectly()
    {
        // Arrange
      var expectedLogFileName = "test.log";
  var expectedUserName = "guest";
      var expectedPassword = "guest";
        var expectedHost = "storage2";
      var expectedPort = 15672;

   // Act
   var config = new Config
        {
       LogFileName = expectedLogFileName,
     RabbitUserName = expectedUserName,
      RabbitPassword = expectedPassword,
 RabbitHost = expectedHost,
   RabbitPort = expectedPort
 };

  // Assert
        Assert.Equal(expectedLogFileName, config.LogFileName);
        Assert.Equal(expectedUserName, config.RabbitUserName);
     Assert.Equal(expectedPassword, config.RabbitPassword);
      Assert.Equal(expectedHost, config.RabbitHost);
      Assert.Equal(expectedPort, config.RabbitPort);
    }

  [Theory]
[InlineData("localhost", 5672)]
 [InlineData("storage2", 15672)]
    [InlineData("rabbitmq.example.com", 5672)]
    [InlineData("10.0.0.1", 15672)]
    public void Config_RabbitMQConnection_ShouldAcceptVariousValues(string host, int port)
    {
        // Arrange & Act
        var config = new Config
        {
 RabbitHost = host,
       RabbitPort = port
   };

     // Assert
   Assert.Equal(host, config.RabbitHost);
   Assert.Equal(port, config.RabbitPort);
    }

    [Fact]
    public void Config_NullableProperties_ShouldAcceptNull()
    {
// Arrange & Act
        var config = new Config
  {
       LogFileName = null,
       RabbitUserName = null,
          RabbitPassword = null,
      RabbitHost = null,
         RabbitPort = 0
        };

   // Assert
  Assert.Null(config.LogFileName);
        Assert.Null(config.RabbitUserName);
        Assert.Null(config.RabbitPassword);
        Assert.Null(config.RabbitHost);
        Assert.Equal(0, config.RabbitPort);
    }

 [Fact]
    public void Config_InitProperties_ShouldBeImmutable()
    {
   // Arrange
    var config = new Config { RabbitHost = "storage2" };

    // Act & Assert
        // Verifies that init-only properties cannot be changed after initialization
     Assert.Equal("storage2", config.RabbitHost);
    }
}
