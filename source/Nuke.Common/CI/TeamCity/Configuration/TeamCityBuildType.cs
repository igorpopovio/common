// Copyright 2019 Maintainers of NUKE.
// Distributed under the MIT License.
// https://github.com/nuke-build/nuke/blob/master/LICENSE

using System;
using System.Linq;
using Nuke.Common.Utilities;

namespace Nuke.Common.CI.TeamCity.Configuration
{
    public class TeamCityBuildType : TeamCityConfigurationEntity
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public TeamCityBuildTypeVcsRoot VcsRoot { get; set; }
        public bool IsComposite { get; set; }
        public string[] InvokedTargets { get; set; }
        public Partition Partition { get; set; }
        public string PartitionTarget { get; set; }
        public TeamCityConfigurationParameter[] Parameters { get; set; }
        public string[] ArtifactRules { get; set; }
        public TeamCityAgentPlatform Platform { get; set; }

        public TeamCityTrigger[] Triggers { get; set; }
        public TeamCityDependency[] Dependencies { get; set; }

        public override void Write(TeamCityConfigurationWriter writer)
        {
            writer.WriteLine($"object {Id} : BuildType({{");
            using (writer.Indent())
            {
                writer.WriteLine($"name = {Name.DoubleQuote()}");

                if (Description != null)
                    writer.WriteLine($"description = {Description.DoubleQuote()}");

                if (IsComposite)
                    writer.WriteLine("type = Type.COMPOSITE");

                VcsRoot.Write(writer);
                WriteArtifacts(writer);

                if (!IsComposite)
                    WriteSteps(writer);

                WriteParameters(writer);
                WriteTriggers(writer);
                WriteDependencies(writer);
            }

            writer.WriteLine("})");
        }

        public virtual void WriteDependencies(TeamCityConfigurationWriter writer)
        {
            if (!Dependencies?.Any() ?? true)
                return;

            using (writer.WriteBlock("dependencies"))
            {
                foreach (var dependency in Dependencies)
                    dependency.Write(writer);
            }
        }

        public virtual void WriteParameters(TeamCityConfigurationWriter writer)
        {
            if (!Parameters?.Any() ?? true)
                return;

            using (writer.WriteBlock("params"))
            {
                foreach (var parameter in Parameters)
                    parameter.Write(writer);
            }
        }

        public virtual void WriteArtifacts(TeamCityConfigurationWriter writer)
        {
            if (!ArtifactRules?.Any() ?? true)
                return;

            writer.WriteLine("artifactRules = \"\"\"");
            using (writer.Indent())
            {
                foreach (var artifactRule in ArtifactRules)
                    writer.WriteLine(artifactRule);
            }

            writer.WriteLine("\"\"\".trimIndent()");
        }

        private void WriteTriggers(TeamCityConfigurationWriter writer)
        {
            if (!Triggers?.Any() ?? true)
                return;

            using (writer.WriteBlock("triggers"))
            {
                foreach (var trigger in Triggers)
                    trigger.Write(writer);
            }
        }

        public virtual void WriteSteps(TeamCityConfigurationWriter writer)
        {
            using (writer.WriteBlock("steps"))
            {
                var arguments = $"{InvokedTargets.JoinSpace()} --skip";
                if (Partition != null)
                {
                    arguments += $" --partition-{PartitionTarget.SplitCamelHumpsWithSeparator("-")}";
                    arguments += $" {Partition.Part}/{Partition.Total}";
                }

                if (Platform == TeamCityAgentPlatform.Unix)
                    WriteExecStep(writer, arguments);
                else
                    WritePowerShellStep(writer, arguments);
            }
        }

        public virtual void WriteExecStep(TeamCityConfigurationWriter writer, string arguments)
        {
            using (writer.WriteBlock("exec"))
            {
                writer.WriteLine("path = \"build.sh\"");
                writer.WriteLine($"arguments = \"{arguments}\"");
            }
        }

        public virtual void WritePowerShellStep(TeamCityConfigurationWriter writer, string arguments)
        {
            using (writer.WriteBlock("powerShell"))
            {
                writer.WriteLine("scriptMode = file { path = \"build.ps1\" }");
                writer.WriteLine($"param(\"jetbrains_powershell_scriptArguments\",\"{arguments}\")");
                writer.WriteLine("noProfile = true");
            }
        }
    }
}
