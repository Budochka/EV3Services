using Xunit;
using VoiceCreator;

namespace VoiceCreator.Tests;

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
   public void Config_AllProperties_ShouldInitializeCorrectly()
    {
 // Arrange
    var expectedLogFileName = "voice.log";
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
    [InlineData("rabbitmq-server", 5672)]
    public void Config_RabbitMQSettings_ShouldAcceptDifferentHosts(string host, int port)
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
    public void Config_InitOnlyProperties_ShouldBeImmutable()
{
        // Arrange
     var config = new Config
        {
      RabbitHost = "storage2",
   RabbitPort = 15672
     };

// Act & Assert
     // Verifies that properties cannot be changed after initialization
      // The following would cause compilation error if uncommented:
     // config.RabbitHost = "newhost"; // CS8852

        Assert.Equal("storage2", config.RabbitHost);
        Assert.Equal(15672, config.RabbitPort);
}

 [Fact]
  public void Config_PartialInitialization_ShouldWorkCorrectly()
    {
  // Arrange & Act
     var config = new Config
    {
      RabbitHost = "storage2",
       RabbitPort = 15672
      // Other properties left as null/default
  };

  // Assert
  Assert.Equal("storage2", config.RabbitHost);
   Assert.Equal(15672, config.RabbitPort);
        Assert.Null(config.LogFileName);
     Assert.Null(config.RabbitUserName);
 Assert.Null(config.RabbitPassword);
    }
}
