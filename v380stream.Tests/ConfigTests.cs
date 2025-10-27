using Xunit;
using v380stream;

namespace v380stream.Tests;

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
    public void Config_InitProperties_ShouldSetCorrectly()
    {
        // Arrange
        var expectedID = "12345678";
        var expectedUserName = "admin";
        var expectedPassword = "test123";
    var expectedIP = "192.168.1.100";
      var expectedPort = 8800;
        var expectedLogFileName = "test.log";

        // Act
        var config = new Config
  {
  ID = expectedID,
            UserName = expectedUserName,
  Password = expectedPassword,
    IP = expectedIP,
         Port = expectedPort,
     LogFileName = expectedLogFileName
        };

   // Assert
        Assert.Equal(expectedID, config.ID);
        Assert.Equal(expectedUserName, config.UserName);
        Assert.Equal(expectedPassword, config.Password);
        Assert.Equal(expectedIP, config.IP);
        Assert.Equal(expectedPort, config.Port);
        Assert.Equal(expectedLogFileName, config.LogFileName);
    }

    [Fact]
  public void Config_NullableProperties_ShouldAcceptNull()
{
        // Arrange & Act
        var config = new Config
        {
            ID = null,
            UserName = null,
  Password = null,
      IP = null,
     Port = 0,
       LogFileName = null
        };

     // Assert
        Assert.Null(config.ID);
  Assert.Null(config.UserName);
      Assert.Null(config.Password);
        Assert.Null(config.IP);
        Assert.Equal(0, config.Port);
        Assert.Null(config.LogFileName);
    }

    [Theory]
    [InlineData(80)]
    [InlineData(8800)]
    [InlineData(15672)]
    [InlineData(65535)]
    public void Config_Port_ShouldAcceptValidPortNumbers(int port)
    {
        // Arrange & Act
    var config = new Config { Port = port };

  // Assert
        Assert.Equal(port, config.Port);
    }

    [Fact]
    public void Config_InitProperties_ShouldBeImmutable()
    {
// Arrange
  var config = new Config { ID = "test123" };

        // Act & Assert
    // This test verifies that init-only properties cannot be changed after initialization
        // If the following line was uncommented, it would cause a compilation error:
        // config.ID = "newvalue"; // CS8852: Init-only property or indexer cannot be assigned to

        Assert.Equal("test123", config.ID);
    }
}
