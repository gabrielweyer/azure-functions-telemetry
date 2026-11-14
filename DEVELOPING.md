# Developing

## Building locally

Run this command to build on Windows:

```powershell
.\build.ps1
```

Run this command to build on Linux / macOS:

```shell
./build.sh
```

Add `--package` to also create the NuGet package in `artifacts/out/package`.

The integration tests need an Application Insights instance and a Service Bus namespace, see [CONTRIBUTING.md](CONTRIBUTING.md).

## Releasing

### How versioning works

- The version is computed by [MinVer][minver] from the most recent version tag reachable from `HEAD`.
- Without a tag on `HEAD`, the version is a prerelease of the next patch: the latest tag with its patch number incremented, followed by `-alpha.0.<number of commits since the tag>` (e.g. `4.0.1` → `4.0.2-alpha.0.36`).
- Every push to `main` builds and **releases**: the workflow creates a GitHub release named `<version>` and pushes the package to NuGet. It also deploys the sample Functions to Azure.
- The package is pushed to NuGet with [Trusted Publishing][trusted-publishing]: the `NuGet/login` action exchanges a GitHub OIDC token for an API key valid for one hour. This requires:
  - A Trusted Publishing policy on nuget.org for this repository and the workflow file `build.yml`. Renaming the workflow file breaks publishing until the policy is updated.
  - A `NUGET_USER` repository secret containing the nuget.org username (profile name, not the email address) that owns the policy.
- No release is created when the `HEAD` commit message contains `[skip-release]` or `dependabot[bot]`.
- Changes that only touch `README.md`, `CONTRIBUTING.md`, `DEVELOPING.md`, `LICENSE`, `docs/`, `.vscode/` or the other workflows don't run the build at all, so they don't release.

### Prerelease from `main`

Nothing to do: merge to `main` and a prerelease (e.g. `4.0.2-alpha.0.36`) is published.

### Stable release (patch, minor or major)

A stable version is only published when `HEAD` is tagged. Tags have **no** prefix: `4.1.0`, **not** `v4.1.0` (MinVer ignores tags that aren't a valid version).

The tag must be on the exact commit that ends up at the tip of `main`. GitHub's merge button breaks this (squash and rebase rewrite commits, a merge commit adds one on top), so merge locally instead:

1. Get the Pull Request green.
2. Tag the last commit of the branch with an annotated tag:

   ```shell
   git tag -a 4.1.0 -m "4.1.0 <summary of the change>"
   ```

3. Fast-forward `main` to the branch:

   ```shell
   git switch main
   git merge --ff-only <branch>
   ```

4. Push `main` and the tag together, so that the build sees the tag:

   ```shell
   git push --atomic origin main 4.1.0
   ```

GitHub closes the Pull Request as merged. The release workflow reuses the existing tag for the GitHub release.

If you add a commit after tagging, move the tag to the new last commit (`git tag -a -f 4.1.0 -m "..."`) before pushing, otherwise the release is `4.1.1-alpha.0.1`.

### Fixing a wrong tag

If you created the wrong tag (e.g. `v4.1.0` instead of `4.1.0`) and haven't pushed `main` yet:

```shell
git tag -a 4.1.0 -m "4.1.0 <summary>" v4.1.0^{}
git tag -d v4.1.0
git push origin :refs/tags/v4.1.0
git push --atomic origin main 4.1.0
```

Pushing a tag alone doesn't trigger a build, so a wrong tag pushed on its own is harmless until `main` is pushed.

### Prereleases from a Pull Request

A Pull Request from a `features/<name>` branch publishes a GitHub prerelease **and** pushes it to NuGet, unless the `HEAD` commit message contains `[skip-release]`. Pull Requests from forks can't publish, as they don't get an OIDC token or the `NUGET_USER` secret.

[minver]: https://github.com/adamralph/minver
[trusted-publishing]: https://learn.microsoft.com/en-us/nuget/nuget-org/trusted-publishing
