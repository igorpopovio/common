// Copyright 2019 Maintainers of NUKE.
// Distributed under the MIT License.
// https://github.com/nuke-build/nuke/blob/master/LICENSE

using System;
using System.Collections.Generic;
using Nuke.Common.Execution;

namespace Nuke.Common.CI.GitHubActions
{
    public enum GitHubActionsTrigger
    {
        Push,
        PullRequest
    }

    public enum GitHubActionsVirtualEnvironments
    {
        WindowsServer2019,
        WindowsServer2016R2,
        Ubuntu1804,
        Ubuntu1604,
        MacOs1014,
        WindowsLatest = WindowsServer2019,
        UbuntuLatest = Ubuntu1804,
        MacOsLatest = MacOs1014,
    }

    public class GitHubActionsAttribute : ConfigurationGenerationAttributeBase
    {
        protected override HostType HostType => HostType.GitHubActions;

        public GitHubActionsVirtualEnvironments[] RunsOn { get; set; }

        public GitHubActionsTrigger[] OnPush { get; set; }
        public string[] OnPushBranches { get; set; }
        public string[] OnPushTags { get; set; }
        public string[] OnPushIncludePaths { get; set; }
        public string[] OnPushExcludePaths { get; set; }
        public string OnCronSchedule { get; set; }

        protected override void Generate(
            NukeBuild build,
            IReadOnlyCollection<ExecutableTarget> executableTargets)
        {
        }
    }
}
