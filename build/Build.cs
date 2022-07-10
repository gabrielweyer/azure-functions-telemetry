using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.ReportGenerator;
using Nuke.Common.Utilities.Collections;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

[ShutdownDotNetAfterServerBuild]
class Build : NukeBuild
{
    /// Support plugins are available for:
    ///   - JetBrains ReSharper        https://nuke.build/resharper
    ///   - JetBrains Rider            https://nuke.build/rider
    ///   - Microsoft VisualStudio     https://nuke.build/visualstudio
    ///   - Microsoft VSCode           https://nuke.build/vscode

    public static int Main() => Execute<Build>(x => x.Publish);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [Parameter("Whether we publish the sample Functions to file system - Default is false")]
    readonly bool Package;

    [Parameter]
    readonly string ServiceBusConnection;

    [Solution] readonly Solution Solution;

    static AbsolutePath SourceDirectory => RootDirectory / "src";
    static AbsolutePath SamplesDirectory => RootDirectory / "samples";
    static AbsolutePath TestsDirectory => RootDirectory / "tests";
    static AbsolutePath ArtifactsDirectory => RootDirectory / "artifacts";
    static AbsolutePath TestResultsDirectory => ArtifactsDirectory / "test-results";
    static AbsolutePath CodeCoverageDirectory => ArtifactsDirectory / "coverage-report";
    static AbsolutePath PublishDirectory => ArtifactsDirectory / "out";
    static int _startedFunctionCount;
    static StringBuilder _defaultV4Output = new();
    static IProcess _defaultV4Function;
    static StringBuilder _customV4Output = new();
    static IProcess _customV4Function;

#pragma warning disable CA1822 // Can't make this static as it breaks NUKE
    Target Clean => _ => _
#pragma warning restore CA1822
        .Executes(() =>
        {
            SourceDirectory.GlobDirectories("**/bin", "**/obj").ForEach(DeleteDirectory);
            SamplesDirectory.GlobDirectories("**/bin", "**/obj").ForEach(DeleteDirectory);
            TestsDirectory.GlobDirectories("**/bin", "**/obj").ForEach(DeleteDirectory);
            EnsureCleanDirectory(ArtifactsDirectory);
        });

    Target Restore => _ => _
        .DependsOn(Clean)
        .Executes(() =>
        {
            DotNetRestore(s => s
                .SetProjectFile(Solution));
        });

    Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
            DotNetBuild(s => s
                .SetProjectFile(Solution)
                .SetConfiguration(Configuration)
                .EnableNoRestore());
        });

    Target VerifyFormat => _ => _
        .DependsOn(Compile)
        .Executes(() =>
        {
            DotNet("format --verify-no-changes");
        });

    Target UnitTest => _ => _
        .DependsOn(VerifyFormat)
        .Executes(() =>
        {
            var testProjects =
                from testProject in Solution.AllProjects
                where testProject.Name == "AzureFunctionsTelemetryTests"
                from framework in testProject.GetTargetFrameworks()
                select new { TestProject = testProject, Framework = framework };

            DotNetTest(s => s
                .SetConfiguration(Configuration)
                .SetDataCollector("XPlat Code Coverage")
                .EnableNoBuild()
                .CombineWith(testProjects, (ss, p) =>
                {
                    var testResultsName = $"{p.TestProject.Path.NameWithoutExtension}-{p.Framework}";
                    var testResultsDirectory = TestResultsDirectory / testResultsName;

                    var loggers = new List<string> { $"html;LogFileName={testResultsName}.html" };

                    if (IsServerBuild)
                    {
                        loggers.Add($"GitHubActions;annotations.titleFormat=$test ({p.Framework})");
                    }

                    return ss
                        .SetProjectFile(p.TestProject)
                        .SetFramework(p.Framework)
                        .SetResultsDirectory(testResultsDirectory)
                        .SetLoggers(loggers);
                }), completeOnFailure: true);
        });

    Target GenerateCoverage => _ => _
        .DependsOn(UnitTest)
        .Executes(() =>
        {
            ReportGeneratorTasks.ReportGenerator(s => s
                .SetFramework("net6.0")
                .SetReports($"{TestResultsDirectory}/**/coverage.cobertura.xml")
                .SetTargetDirectory(CodeCoverageDirectory)
                .SetReportTypes(ReportTypes.Html));
        });

    Target StartAzureFunctions => _ => _
        .DependsOn(GenerateCoverage)
        .OnlyWhenDynamic(() => ShouldWeRunIntegrationTests())
        .Executes(() =>
        {
            var slim = new ManualResetEventSlim();
            Serilog.Log.Information("Starting Azure Functions, this takes some time");

            _defaultV4Output = new StringBuilder();
            var defaultV4Logger = BuildLogger(_defaultV4Output, "DefaultV4InProcessFunction");

            _customV4Output = new StringBuilder();
            var customV4Logger = BuildLogger(_customV4Output, "CustomV4InProcessFunction");

            var environmentVariables = new Dictionary<string, string>(EnvironmentInfo.Variables)
            {
                ["Testing__IsEnabled"] = "true",
                ["Testing:IsEnabled"] = "true"
            };

            _defaultV4Function = StartFunction("DefaultV4InProcessFunction", defaultV4Logger);
            _customV4Function = StartFunction("CustomV4InProcessFunction", customV4Logger);

            var functionsTimeoutStart = TimeSpan.FromSeconds(60);
            var didFunctionsStart = slim.Wait(functionsTimeoutStart);

            if (didFunctionsStart)
            {
                return;
            }

            throw new InvalidOperationException(
                $"Failed to start DefaultV4InProcessFunction or CustomV4InProcessFunction within {functionsTimeoutStart}");

            Action<OutputType, string> BuildLogger(StringBuilder sink, string functionName)
            {
                return (outputType, output) =>
                {
                    sink.AppendLine($"[{outputType}] - {output}");

                    if (!output.Contains("Job host started"))
                    {
                        return;
                    }

                    Serilog.Log.Information($"Started {functionName}");
                    var startedFunctionCount = Interlocked.Increment(ref _startedFunctionCount);

                    if (startedFunctionCount > 1)
                    {
                        slim.Set();
                    }
                };
            }

            IProcess StartFunction(string functionName, Action<OutputType, string> logger)
            {
                return ProcessTasks
                    .StartProcess("func", "start", SamplesDirectory / functionName, environmentVariables, null, null, null, logger);
            }
        });

    Target IntegrationTest => _ => _
        .DependsOn(StartAzureFunctions)
        .OnlyWhenDynamic(() => SucceededTargets.Contains(StartAzureFunctions))
        .Executes(() =>
        {
            const string projectName = "AzureFunctionsTelemetryIntegrationTests";

            var dotnetTestSettings = new DotNetTestSettings()
                .SetConfiguration(Configuration)
                .EnableNoBuild()
                .SetProjectFile(TestsDirectory / projectName)
                .SetResultsDirectory(TestResultsDirectory / projectName)
                .SetLoggers($"html;LogFileName={projectName}.html");
            DotNetTest(dotnetTestSettings);
        });

    Target WriteAzureFunctionLogs => _ => _
        .DependsOn(IntegrationTest)
        .AssuredAfterFailure()
        .OnlyWhenDynamic(() => FailedTargets.Contains(IntegrationTest) || FailedTargets.Contains(StartAzureFunctions))
        .Executes(() =>
        {
            var defaultV4OutputPath = TestResultsDirectory / "default-v4-output.log";
            var customV4OutputPath = TestResultsDirectory / "custom-v4-output.log";
            Serilog.Log.Error($"Wrote DefaultV4InProcessFunction logs to: {defaultV4OutputPath}");
            Serilog.Log.Error($"Wrote CustomV4InProcessFunction logs to: {customV4OutputPath}");
            File.WriteAllText(defaultV4OutputPath, _defaultV4Output.ToString());
            File.WriteAllText(customV4OutputPath, _customV4Output.ToString());
        });

    Target StopAzureFunctions => _ => _
        .DependsOn(WriteAzureFunctionLogs)
        .AssuredAfterFailure()
        .OnlyWhenDynamic(() => SucceededTargets.Contains(StartAzureFunctions))
        .Executes(() =>
        {
            Serilog.Log.Information("Stopping Azure Functions");

            _defaultV4Function.Kill();
            _customV4Function.Kill();
        });

    Target Publish => _ => _
        .DependsOn(StopAzureFunctions)
        .OnlyWhenStatic(() => Package)
        .Executes(() =>
        {
            var functionProjects =
                from functionProject in Solution.AllProjects
                where functionProject.Name.EndsWith("Function")
                select functionProject;

            var outDirectories = new List<(string ProjectName, string OutPath)>();

            DotNetPublish(s => s
                .SetConfiguration(Configuration)
                .EnableNoBuild()
                .CombineWith(functionProjects, (ss, p) =>
                {
                    var output = PublishDirectory / p.Name;
                    outDirectories.Add((p.Name, output));

                    return ss
                        .SetProject(p)
                        .SetOutput(output);
                }));

            foreach (var (projectName, outPath) in outDirectories)
            {
                CompressionTasks.CompressZip(outPath, PublishDirectory / $"{projectName}.zip");
            }
        });

    bool ShouldWeRunIntegrationTests()
    {
        var shouldWe = IsServerBuild || !string.IsNullOrWhiteSpace(ServiceBusConnection);

        if (!shouldWe)
        {
            Serilog.Log.Warning("Skipping integration tests because 'ServiceBusConnection' environment variable not set");
        }

        return shouldWe;
    }
}
