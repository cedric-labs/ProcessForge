using FluentAssertions;
using ProcessForge.Filtering;
using ProcessForge.Inspectors;
using ProcessForge.Models;
using Xunit;

namespace ProcessForge.Tests;

public sealed class ProcessForgeTests
{
    [Theory]
    [InlineData("java", "java -jar /opt/app/myservice.jar --port 8080", "/opt/app/myservice.jar")]
    [InlineData("cmd", "cmd.exe /c \"C:\\scripts\\run_worker.bat\" arg1", "C:\\scripts\\run_worker.bat")]
    [InlineData("powershell", "powershell.exe -ExecutionPolicy Bypass -File C:\\tools\\audit.ps1", "C:\\tools\\audit.ps1")]
    [InlineData("bash", "/bin/bash /usr/local/bin/backup.sh", "/usr/local/bin/backup.sh")]
    public void ResolveTargetPayload_WhenHostProcessHasTarget_ExtractsTarget(string procName, string cmdLine, string expected)
    {
        // Act
        var target = CrossPlatformProcessInspector.ResolveTargetPayload(procName, cmdLine);

        // Assert
        target.Should().Be(expected);
    }

    [Fact]
    public void ShouldExclude_WhenPidMatches_ReturnsTrue()
    {
        // Arrange
        var filter = new ProcessExclusionFilter(new ProcessExclusionOptions { ExcludedProcessIds = [4] });
        var proc = new ProcessMetadata(4, "System", "", "", "", "SYSTEM");

        // Act & Assert
        filter.ShouldExclude(proc).Should().BeTrue();
    }
}
