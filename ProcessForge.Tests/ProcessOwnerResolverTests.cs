using AwesomeAssertions;
using ProcessForge.Inspectors;
using Xunit;

namespace ProcessForge.Tests;

public sealed class ProcessOwnerResolverTests
{
    [Fact]
    public async Task ResolveProcessAsync_WhenProcessIsCurrentProcess_ReturnsCurrentAccount()
    {
        // Arrange
        var inspector = new CrossPlatformProcessInspector();

        // Act
        var metadata = await inspector.ResolveProcessAsync(Environment.ProcessId);

        // Assert
        metadata.UserName.Should().EndWith(Environment.UserName);
    }

    [Fact]
    public void ParseRealUserId_WhenStatusContainsUidLine_ReturnsRealUserId()
    {
        // Arrange
        const string status = "Name:\tnginx\nState:\tS (sleeping)\nUid:\t33\t33\t33\t33\nGid:\t33\t33\t33\t33\n";

        // Act
        var uid = ProcessOwnerResolver.ParseRealUserId(status);

        // Assert
        uid.Should().Be(33);
    }

    [Fact]
    public void ParseRealUserId_WhenUidLineIsMissing_ReturnsNull()
    {
        // Act
        var uid = ProcessOwnerResolver.ParseRealUserId("Name:\tnginx\nState:\tS (sleeping)\n");

        // Assert
        uid.Should().BeNull();
    }

    [Fact]
    public void FindUserName_WhenUserIdIsListed_ReturnsAccountName()
    {
        // Arrange
        const string passwd = "root:x:0:0:root:/root:/bin/bash\nwww-data:x:33:33:www-data:/var/www:/usr/sbin/nologin\n";

        // Act
        var name = ProcessOwnerResolver.FindUserName(passwd, 33);

        // Assert
        name.Should().Be("www-data");
    }

    [Fact]
    public void FindUserName_WhenUserIdIsNotListed_ReturnsNumericUserId()
    {
        // Act
        var name = ProcessOwnerResolver.FindUserName("root:x:0:0:root:/root:/bin/bash\n", 1234);

        // Assert
        name.Should().Be("1234");
    }
}
