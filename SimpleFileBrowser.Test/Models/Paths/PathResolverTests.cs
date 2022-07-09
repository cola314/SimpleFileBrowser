using SimpleFileBrowser.Models.ApplicationEnvironments;
using SimpleFileBrowser.Models.Paths;

namespace SimpleFileBrowser.Test.Models.Paths;

public class PathResolverTests
{
    class TestEnvironment : ApplicationEnvironment
    {
        public TestEnvironment(string storagePath)
        {
            StoragePath = storagePath;
        }
    }

    [Theory]
    [InlineData(@"c:\foo", @"c:", true)]
    [InlineData(@"c:\foo", @"c:\", true)]
    [InlineData(@"c:\foo", @"c:\foo", false)]
    [InlineData(@"c:\foo", @"c:\foo\", false)]
    [InlineData(@"c:\foo\", @"c:\foo", false)]
    [InlineData(@"c:\foo\", @"c:\bar", false)]
    [InlineData(@"c:\foo\bar\", @"c:\foo\", true)]
    [InlineData(@"c:\foo\bar", @"c:\foo\", true)]
    [InlineData(@"c:\foo\a.txt", @"c:\foo", true)]
    [InlineData(@"c:\FOO\a.txt", @"c:\foo", true)]
    [InlineData(@"c:/foo/a.txt", @"c:\foo", true)]
    [InlineData(@"c:\foobar", @"c:\foo", false)]
    [InlineData(@"c:\foobar\a.txt", @"c:\foo", false)]
    [InlineData(@"c:\foobar\a.txt", @"c:\foo\", false)]
    [InlineData(@"c:\foo\a.txt", @"c:\foobar", false)]
    [InlineData(@"c:\foo\a.txt", @"c:\foobar\", false)]
    [InlineData(@"c:\foo\..\bar\baz", @"c:\foo", false)]
    [InlineData(@"c:\foo\..\bar", @"c:\foo", false)]
    [InlineData(@"c:\foo\..\bar\baz", @"c:\bar", true)]
    [InlineData(@"c:\foo\..\bar\baz", @"c:\barr", false)]
    public void ValidatePathTest(string path, string storagePath, bool expectedResult)
    {
        var env = new TestEnvironment(storagePath);
        var resolver = new PathResolver(env);

        bool result = false;
        try
        {
            resolver.Resolve(path);
            result = true;
        }
        catch (InvalidPathException)
        {
        }
        Assert.Equal(expectedResult, result);
    }
}