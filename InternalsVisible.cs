using System.Runtime.CompilerServices;

// The test project exercises engine internals directly. A ProjectReference from a test library
// to a WinExe is legal and gives the same coverage without restructuring the repository.
[assembly: InternalsVisibleTo("CodeShuttle.Tests")]
