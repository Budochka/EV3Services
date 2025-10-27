using Xunit;
using Logger;

namespace Logger.Tests;

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
    public void Config_AllProperties_ShouldSetAndGetCorrectly()
{
        // Arrange
  var expectedLogFileName = "logger.log";
     var expectedUserName = "guest";
  var expectedPassword = "guest";
  var expectedHost = "storage2";
  var expectedPort = 15672;
var expectedConnectionString = "Host=storage;Port=3307;User ID=ev3;Password=ev3;Database=ev3";

// Act
        var config = new Config
        {
       LogFileName = expectedLogFileName,
            RabbitUserName = expectedUserName,
       RabbitPassword = expectedPassword,
         RabbitHost = expectedHost,
     RabbitPort = expectedPort,
     ConnectionString = expectedConnectionString
        };

        // Assert
    Assert.Equal(expectedLogFileName, config.LogFileName);
      Assert.Equal(expectedUserName, config.RabbitUserName);
   Assert.Equal(expectedPassword, config.RabbitPassword);
  Assert.Equal(expectedHost, config.RabbitHost);
        Assert.Equal(expectedPort, config.RabbitPort);
     Assert.Equal(expectedConnectionString, config.ConnectionString);
    }

 [Fact]
    public void Config_RabbitMQProperties_ShouldBeSettable()
{
    // Arrange
     var config = new Config();

// Act
  config.RabbitHost = "storage2";
 config.RabbitPort = 15672;
   config.RabbitUserName = "admin";
  config.RabbitPassword = "secret";

  // Assert
        Assert.Equal("storage2", config.RabbitHost);
      Assert.Equal(15672, config.RabbitPort);
        Assert.Equal("admin", config.RabbitUserName);
     Assert.Equal("secret", config.RabbitPassword);
    }

  [Fact]
 public void Config_ConnectionString_ShouldBeSettable()
    {
        // Arrange
  var config = new Config();
   var expectedConnectionString = "Host=localhost;Database=testdb";

     // Act
        config.ConnectionString = expectedConnectionString;

 // Assert
     Assert.Equal(expectedConnectionString, config.ConnectionString);
    }

  [Fact]
    public void Config_Properties_ShouldAcceptNull()
 {
 // Arrange
   var config = new Config
  {
    LogFileName = null,
   RabbitUserName = null,
   RabbitPassword = null,
  RabbitHost = null,
     RabbitPort = 0,
 ConnectionString = null
  };

 // Assert
     Assert.Null(config.LogFileName);
        Assert.Null(config.RabbitUserName);
        Assert.Null(config.RabbitPassword);
      Assert.Null(config.RabbitHost);
        Assert.Equal(0, config.RabbitPort);
   Assert.Null(config.ConnectionString);
    }

 [Theory]
    [InlineData("Host=db1;Port=3306;Database=db1")]
    [InlineData("Host=storage;Port=3307;User ID=ev3;Password=ev3;Database=ev3")]
  [InlineData("Server=localhost;Database=testdb;Trusted_Connection=True")]
    public void Config_ConnectionString_ShouldAcceptVariousFormats(string connectionString)
    {
     // Arrange & Act
     var config = new Config { ConnectionString = connectionString };

     // Assert
        Assert.Equal(connectionString, config.ConnectionString);
 }

   [Fact]
    public void Config_ModifyProperties_ShouldUpdateValues()
{
   // Arrange
    var config = new Config { RabbitHost = "localhost" };

        // Act
config.RabbitHost = "storage2";

// Assert
  Assert.Equal("storage2", config.RabbitHost);
    }
}
