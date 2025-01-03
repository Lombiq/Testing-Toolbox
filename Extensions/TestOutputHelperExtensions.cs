using Lombiq.Tests.Helpers;

namespace Xunit;

public static class TestOutputHelperExtensions
{
    /// <summary>
    /// Creates a <see langword="string"/> from <paramref name="format"/> and <paramref name="args"/>, prepends a time
    /// stamp, then outputs it via both <see cref="DebugHelper"/> and <paramref name="testOutputHelper"/>.
    /// </summary>
    public static void WriteLineTimestampedAndDebug(this ITestOutputHelper testOutputHelper, string format, params object[] args)
    {
        testOutputHelper.WriteLineTimestamped(format, args);
        DebugHelper.WriteLineTimestamped(format, args);
    }

    /// <summary>
    /// Creates a <see langword="string"/> from <paramref name="format"/> and <paramref name="args"/>, prepends a time
    /// stamp, then outputs it via <paramref name="testOutputHelper"/>.
    /// </summary>
    public static void WriteLineTimestamped(this ITestOutputHelper testOutputHelper, string format, params object[] args)
    {
        // Preventing "FormatException : Input string was not in a correct format." exceptions if the message contains
        // characters used in string formatting, but it shouldn't actually be formatted.
        if (args == null || args.Length == 0) testOutputHelper.WriteLine(DebugHelper.PrefixWithTimestamp(format));
        else testOutputHelper.WriteLine(DebugHelper.PrefixWithTimestamp(format), args);
    }

    /// <summary>
    /// A shortcut of <see cref="WriteLineTimestampedAndDebug"/> for outputting output from another source as-is. It's
    /// prefixed with a timestamp and <paramref name="name"/>.
    /// </summary>
    /// <param name="name">The name of the source (command, method, etc) that generated this line.</param>
    /// <param name="output">The text to be logged.</param>
    public static void WriteOutputTimestampedAndDebug(this ITestOutputHelper testOutputHelper, string name, string output) =>
        testOutputHelper.WriteLineTimestampedAndDebug("{0}: {1}", name, output);
}
