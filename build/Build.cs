using System;
using System.Collections.Generic;
using System.Globalization;
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
using Nuke.Common.Tools.MinVer;
using Nuke.Common.Tools.ReportGenerator;
using Nuke.Common.Utilities;
using Nuke.Common.Utilities.Collections;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

namespace Gabo;

[ShutdownDotNetAfterServerBuild]
sealed class Build : NukeBuild
{
    /// Support plugins are available for:
    ///   - JetBrains ReSharper        https://nuke.build/resharper
    ///   - JetBrains Rider            https://nuke.build/rider
    ///   - Microsoft VisualStudio     https://nuke.build/visualstudio
    ///   - Microsoft VSCode           https://nuke.build/vscode

    public static int Main() => Execute<Build>(x => x.PublishFunctions);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [Parameter("Whether we publish the sample Functions to file system - Default is false")]
    readonly bool Package;

    [Parameter]
    readonly string IntegrationTestServiceBusConnectionString;

    [Solution] readonly Solution Solution;

    [MinVer]
    readonly MinVer MinVer;

    const string UserSecretsId = "074ca336-270b-4832-9a1a-60baf152b727";
    const string AppInsightsSecretName = "APPLICATIONINSIGHTS_CONNECTION_STRING";
    const string ServiceBusConnectionSecretName = "ServiceBusConnection";
    const string TestingIsEnabledSecretName = "Testing:IsEnabled";
    static string _serviceBusConnectionBackup;
    static string _appInsightsConnectionBackup;
    static AbsolutePath SourceDirectory => RootDirectory / "src";
    static AbsolutePath SamplesDirectory => RootDirectory / "samples";
    static AbsolutePath TestsDirectory => RootDirectory / "tests";
    static AbsolutePath ArtifactsDirectory => RootDirectory / "artifacts";
    static AbsolutePath TestResultsDirectory => ArtifactsDirectory / "test-results";
    static AbsolutePath CodeCoverageDirectory => ArtifactsDirectory / "coverage-report";
    static AbsolutePath PublishDirectory => ArtifactsDirectory / "out";
    static AbsolutePath FunctionsPublishDirectory => PublishDirectory / "functions";
    static AbsolutePath PackagePublishDirectory => PublishDirectory / "package";
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

    Target SetGitHubVersion => _ => _
        .DependsOn(Restore)
        .OnlyWhenStatic(() => IsServerBuild)
        .Executes(() =>
        {
            var gitHubEnvironmentFile = Environment.GetEnvironmentVariable("GITHUB_ENV");
            var packageVersionEnvironmentVariable = $"PACKAGE_VERSION={MinVer.PackageVersion}";
            File.WriteAllText(gitHubEnvironmentFile, packageVersionEnvironmentVariable);
        });

    Target Compile => _ => _
        .DependsOn(SetGitHubVersion)
        .DependsOn(Restore)
        .Executes(() =>
        {
            Serilog.Log.Information("AssemblyVersion: {AssemblyVersion}", MinVer.AssemblyVersion);
            Serilog.Log.Information("NuGetVersion: {PackageVersion}", MinVer.PackageVersion);

            DotNetBuild(s => s
                .SetProjectFile(Solution)
                .SetConfiguration(Configuration)
                .SetAssemblyVersion(MinVer.AssemblyVersion)
                .SetFileVersion(MinVer.PackageVersion)
                .SetInformationalVersion(MinVer.PackageVersion)
                .EnableNoRestore()
                .EnableNoIncremental());
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
                where "AzureFunctionsTelemetryTests".Equals(testProject.Name, StringComparison.Ordinal)
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

    Target SetIntegrationTestUserSecrets => _ => _
        .DependsOn(GenerateCoverage)
        .OnlyWhenDynamic(() => ShouldWeRunIntegrationTests())
        .Executes(() =>
        {
            var serviceBusSecret = GetUserSecret(ServiceBusConnectionSecretName);

            if (serviceBusSecret != null)
            {
                Serilog.Log.Debug($"'{ServiceBusConnectionSecretName}' secret is present, keeping backup");
                _serviceBusConnectionBackup = serviceBusSecret;
            }

            SetUserSecret(TestingIsEnabledSecretName, "true");
            SetUserSecret(ServiceBusConnectionSecretName, IntegrationTestServiceBusConnectionString);
        });

    Target SetAppInsightsConnectionStringIntegrationTestUserSecrets => _ => _
        .DependsOn(SetIntegrationTestUserSecrets)
        .OnlyWhenDynamic(() => SucceededTargets.Contains(SetIntegrationTestUserSecrets))
        .Executes(() =>
        {
            var appInsightsSecret = GetUserSecret(AppInsightsSecretName);

            if (appInsightsSecret == null)
            {
                return;
            }

            Serilog.Log.Debug($"'{AppInsightsSecretName}' secret is present, keeping backup");
            _appInsightsConnectionBackup = appInsightsSecret;
            Serilog.Log.Debug($" Removing'{AppInsightsSecretName}' secret");
            RemoveUserSecret(AppInsightsSecretName);
        });

    Target StartAppInsightsConnectionStringIntegrationTestAzureFunctions => _ => _
        .DependsOn(SetAppInsightsConnectionStringIntegrationTestUserSecrets)
        .OnlyWhenDynamic(() => SucceededTargets.Contains(SetAppInsightsConnectionStringIntegrationTestUserSecrets))
        .Executes(StartAzureFunctions);

    Target AppInsightsConnectionStringIntegrationTest => _ => _
        .DependsOn(StartAppInsightsConnectionStringIntegrationTestAzureFunctions)
        .OnlyWhenDynamic(() => SucceededTargets.Contains(StartAppInsightsConnectionStringIntegrationTestAzureFunctions))
        .Executes(() =>
        {
            RunIntegrationTest("AppInsightsConnectionStringIntegrationTests");
        });

    Target WriteAppInsightsConnectionStringIntegrationTestAzureFunctionLogs => _ => _
        .DependsOn(AppInsightsConnectionStringIntegrationTest)
        .AssuredAfterFailure()
        .OnlyWhenDynamic(() =>
            FailedTargets.Contains(AppInsightsConnectionStringIntegrationTest) ||
            FailedTargets.Contains(StartAppInsightsConnectionStringIntegrationTestAzureFunctions))
        .Executes(() =>
        {
            WriteAzureFunctionsLogs("default-v4-appinsights-output", "custom-v4-appinsights-output");
        });

    Target StopAppInsightsConnectionStringIntegrationTestAzureFunctions => _ => _
        .DependsOn(WriteAppInsightsConnectionStringIntegrationTestAzureFunctionLogs)
        .AssuredAfterFailure()
        .OnlyWhenDynamic(() => SucceededTargets.Contains(StartAppInsightsConnectionStringIntegrationTestAzureFunctions))
        .Executes(StopAzureFunctions);

    Target RestoreAppInsightsConnectionStringUserSecrets => _ => _
        .DependsOn(StopAppInsightsConnectionStringIntegrationTestAzureFunctions)
        .AssuredAfterFailure()
        .OnlyWhenDynamic(() => SucceededTargets.Contains(SetAppInsightsConnectionStringIntegrationTestUserSecrets))
        .Executes(() =>
        {
            if (_appInsightsConnectionBackup == null)
            {
                return;
            }

            Serilog.Log.Information($"Restoring '{AppInsightsSecretName}' secret");
            SetUserSecret(AppInsightsSecretName, _appInsightsConnectionBackup);
        });

    Target StartIntegrationTestAzureFunctions => _ => _
        .DependsOn(RestoreAppInsightsConnectionStringUserSecrets)
        .OnlyWhenDynamic(() => SucceededTargets.Contains(SetIntegrationTestUserSecrets))
        .Executes(StartAzureFunctions);

    Target IntegrationTest => _ => _
        .DependsOn(StartIntegrationTestAzureFunctions)
        .OnlyWhenDynamic(() => SucceededTargets.Contains(StartIntegrationTestAzureFunctions))
        .Executes(() =>
        {
            RunIntegrationTest("AzureFunctionsTelemetryIntegrationTests");
        });

    Target WriteIntegrationTestAzureFunctionLogs => _ => _
        .DependsOn(IntegrationTest)
        .AssuredAfterFailure()
        .OnlyWhenDynamic(() =>
            FailedTargets.Contains(IntegrationTest) || FailedTargets.Contains(StartIntegrationTestAzureFunctions))
        .Executes(() =>
        {
            WriteAzureFunctionsLogs("default-v4-output", "custom-v4-output");
        });

    Target StopIntegrationTestAzureFunctions => _ => _
        .DependsOn(WriteIntegrationTestAzureFunctionLogs)
        .AssuredAfterFailure()
        .OnlyWhenDynamic(() => SucceededTargets.Contains(StartIntegrationTestAzureFunctions))
        .Executes(StopAzureFunctions);

    Target RestoreUserSecrets => _ => _
        .DependsOn(StopIntegrationTestAzureFunctions)
        .AssuredAfterFailure()
        .OnlyWhenDynamic(() => SucceededTargets.Contains(SetIntegrationTestUserSecrets))
        .Executes(() =>
        {
            RemoveUserSecret(TestingIsEnabledSecretName);

            if (_serviceBusConnectionBackup != null)
            {
                Serilog.Log.Information($"Restoring '{ServiceBusConnectionSecretName}' secret");
                SetUserSecret(ServiceBusConnectionSecretName, _serviceBusConnectionBackup);
            }
            else
            {
                Serilog.Log.Information("Removing integration test Service Bus Connecting String");
                RemoveUserSecret(ServiceBusConnectionSecretName);
            }
        });

    Target Pack => _ => _
        .DependsOn(RestoreUserSecrets)
        .OnlyWhenDynamic(() => Package)
        .Executes(() =>
        {
            DotNetPack(s => s
                .SetConfiguration(Configuration)
                .EnableNoBuild()
                .SetProject(SourceDirectory / "AzureFunctionsTelemetry")
                .EnableIncludeSymbols()
                .SetOutputDirectory(PackagePublishDirectory)
                .SetVersion(MinVer.PackageVersion));
        });

    Target PublishFunctions => _ => _
        .DependsOn(Pack)
        .OnlyWhenDynamic(() => Package)
        .Executes(() =>
        {
            var functionProjects =
                from functionProject in Solution.AllProjects
                where functionProject.Name.EndsWith("Function", StringComparison.Ordinal)
                select functionProject;

            var outDirectories = new List<(string ProjectName, string OutPath)>();

            DotNetPublish(s => s
                .SetConfiguration(Configuration)
                .EnableNoBuild()
                .CombineWith(functionProjects, (ss, p) =>
                {
                    var output = FunctionsPublishDirectory / p.Name;
                    outDirectories.Add((p.Name, output));

                    return ss
                        .SetProject(p)
                        .SetOutput(output);
                }));

            foreach (var (projectName, outPath) in outDirectories)
            {
                CompressionTasks.CompressZip(outPath, FunctionsPublishDirectory / $"{projectName}.zip");
            }
        });

    bool ShouldWeRunIntegrationTests()
    {
        var shouldWe = !string.IsNullOrWhiteSpace(IntegrationTestServiceBusConnectionString);

        if (shouldWe)
        {
            return true;
        }

        if (IsLocalBuild)
        {
            Serilog.Log.Warning($"Skipping integration tests because '{nameof(IntegrationTestServiceBusConnectionString)}' environment variable not set");
        }
        else
        {
            Assert.Fail($"'{nameof(IntegrationTestServiceBusConnectionString)}' environment variable should be set to run integration tests");
        }

        return false;
    }

    static string GetUserSecret(string secretName)
    {
        var secretPrefix = $"{secretName} = ";
        var userSecrets = DotNet($"user-secrets list --id {UserSecretsId}", logOutput: false);
        var secret = userSecrets.SingleOrDefault(o => o.Type == OutputType.Std && o.Text.StartsWith(secretPrefix, StringComparison.Ordinal));
        return secret.Text.IsNullOrEmpty() ? null : secret.Text.Replace(secretPrefix, string.Empty, StringComparison.Ordinal);
    }

    static void SetUserSecret(string secretName, string secretValue)
    {
        DotNet(
            $"user-secrets set {secretName} {secretValue} --id {UserSecretsId}",
            outputFilter: o => o.Replace(secretValue, "*****", StringComparison.Ordinal));
    }

    static void RemoveUserSecret(string secretName)
    {
        DotNet($"user-secrets remove {secretName} --id {UserSecretsId}");
    }

    static void StartAzureFunctions()
    {
        using var slim = new ManualResetEventSlim();
        Serilog.Log.Information("Starting Azure Functions, this takes some time");

        _defaultV4Output = new StringBuilder();
        var defaultV4Logger = BuildLogger(_defaultV4Output, "DefaultV4InProcessFunction");

        _customV4Output = new StringBuilder();
        var customV4Logger = BuildLogger(_customV4Output, "CustomV4InProcessFunction");

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
                sink.AppendLine(CultureInfo.InvariantCulture, $"[{outputType}] - {output}");

                if (!output.Contains("Job host started", StringComparison.Ordinal))
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
                .StartProcess("func", "start", SamplesDirectory / functionName, null, null, null, null, logger);
        }
    }

    void RunIntegrationTest(string projectName)
    {
        var loggers = new List<string> { $"html;LogFileName={projectName}.html" };

        if (IsServerBuild)
        {
            loggers.Add("GitHubActions");
        }

        var dotnetTestSettings = new DotNetTestSettings()
            .SetConfiguration(Configuration)
            .EnableNoBuild()
            .SetProjectFile(TestsDirectory / projectName)
            .SetResultsDirectory(TestResultsDirectory / projectName)
            .SetLoggers(loggers);
        DotNetTest(dotnetTestSettings);
    }

    static void WriteAzureFunctionsLogs(string defaultLogName, string customLogName)
    {
        var defaultV4OutputPath = TestResultsDirectory / $"{defaultLogName}.log";
        var customV4OutputPath = TestResultsDirectory / $"{customLogName}.log";
        Serilog.Log.Warning($"Wrote DefaultV4InProcessFunction logs to: {defaultV4OutputPath}");
        Serilog.Log.Warning($"Wrote CustomV4InProcessFunction logs to: {customV4OutputPath}");
        File.WriteAllText(defaultV4OutputPath, _defaultV4Output.ToString());
        File.WriteAllText(customV4OutputPath, _customV4Output.ToString());
    }

    static void StopAzureFunctions()
    {
        Serilog.Log.Information("Stopping Azure Functions");

        _defaultV4Function.Kill();
        _customV4Function.Kill();
    }
}
