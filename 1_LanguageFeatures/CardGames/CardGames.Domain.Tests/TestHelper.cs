using Guts.Client.Core.TestTools;

namespace CardGames.Domain.Tests;

internal static class TestHelper
{
    public static void AssertFileHasNotChanged(string relativeFilePath, string expectedHash)
    {
        var fileHash = Solution.Current.GetFileHash(relativeFilePath);
        Assert.That(fileHash, Is.EqualTo(expectedHash),
            $"The file '{relativeFilePath}' has changed. " +
            "Undo your changes on the file to make this test pass.");
    }
}