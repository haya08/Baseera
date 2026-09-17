namespace Baseera.Infrastructure.Tests.TestSetups
{
    public static class TestEnvironment
    {
        public static void Load()
        {
            var solutionDirectory = Directory.GetParent(
                AppContext.BaseDirectory
            )!.Parent!.Parent!.Parent!.Parent!.FullName;

            var envPath = Path.Combine(solutionDirectory, ".env");

            DotNetEnv.Env.Load(envPath);
        }
    }
}
