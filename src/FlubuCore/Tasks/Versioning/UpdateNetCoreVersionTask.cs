﻿using System;
using System.Collections.Generic;
using FlubuCore.Context;
using FlubuCore.IO.Wrappers;
using FlubuCore.Tasks.Text;

namespace FlubuCore.Tasks.Versioning
{
    public class UpdateNetCoreVersionTask : TaskBase<int, UpdateNetCoreVersionTask>
    {
        private readonly List<string> _files = new List<string>();

        private readonly IPathWrapper _pathWrapper;

        private readonly IFileWrapper _file;

        private BuildVersion _version;
        private string _description;

        public UpdateNetCoreVersionTask(IPathWrapper pathWrapper, IFileWrapper filWrapper, string file)
        {
            _file = filWrapper;
            _pathWrapper = pathWrapper;
            _files.Add(file);
        }

        protected override string Description
        {
            get
            {
                if (string.IsNullOrEmpty(_description))
                {
                    return $"Updates version, AssemblyVersion and FileVersion in csproj/project.json file.";
                }

                return _description;
            }

            set => _description = value;
        }

        internal List<string> AdditionalProperties { get; } = new List<string>();

        /// <summary>
        /// Use fixed version instead of fetching one from <see cref="IBuildPropertiesSession"/> build property named: <see cref="BuildProps.BuildVersion"/>
        /// </summary>
        /// <param name="version"></param>
        /// <returns></returns>
        [Obsolete("Use set version instead")]
        public UpdateNetCoreVersionTask FixedVersion(BuildVersion version)
        {
            _version = version;
            return this;
        }

        /// <summary>
        /// Use fixed version instead of fetching one from <see cref="IBuildPropertiesSession"/> build property named: <see cref="BuildProps.BuildVersion"/>
        /// </summary>
        /// <param name="version"></param>
        /// <returns></returns>
        [Obsolete]
        public UpdateNetCoreVersionTask SetVersion(BuildVersion version)
        {
            _version = version;
            return this;
        }

        /// <summary>
        /// Adds additional properties to be updated with the version.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public UpdateNetCoreVersionTask AdditionalProp(params string[] args)
        {
            if (args == null || args.Length <= 0)
                return this;

            AdditionalProperties.AddRange(args);
            return this;
        }

        /// <summary>
        /// Adds Project (json/cproj) files to be updated.
        /// </summary>
        /// <param name="files"></param>
        /// <returns></returns>
        public UpdateNetCoreVersionTask AddFiles(params string[] files)
        {
            _files.AddRange(files);
            return this;
        }

        protected override int DoExecute(ITaskContextInternal context)
        {
            if (_version == null)
            {
                _version = new BuildVersion();
                _version.Version = context.Properties.GetBuildVersion();
                _version.VersionQuality = context.Properties.GetBuildVersionQuality();
            }

            if (_version == null || _version.Version == null)
            {
                throw new TaskExecutionException("Version is not set!", 1);
            }

            DoLogInfo($"Update version to {_version.Version}");
            string newVersion = _version.Version.ToString(3);
            int res = 0;

            foreach (string file in _files)
            {
                if (string.IsNullOrEmpty(file))
                {
                    continue;
                }

                if (!_file.Exists(file))
                {
                    context.Fail($"File {file} not found!", 1);
                    return 1;
                }

                string newVersionWithQuality = newVersion;
                if (!string.IsNullOrEmpty(_version.VersionQuality))
                {
                    if (!_version.VersionQuality.StartsWith("-"))
                    {
                        _version.VersionQuality = _version.VersionQuality.Insert(0, "-");
                        newVersionWithQuality = $"{newVersionWithQuality}{_version.VersionQuality}";
                    }
                }

                if (_pathWrapper.GetExtension(file).Equals(".xproj", StringComparison.CurrentCultureIgnoreCase))
                {
                    UpdateJsonFileTask task = context.Tasks().UpdateJsonFileTask(file);

                    task
                        .FailIfPropertyNotFound(false)
                        .Update("version", newVersionWithQuality);

                    AdditionalProperties.ForEach(i => task.Update(i, newVersion));

                    task.Execute(context);
                }
                else
                {
                    var task = context.Tasks().UpdateXmlFileTask(file);
                    task.AddOrUpdate("Project/PropertyGroup/Version", newVersionWithQuality);
                    newVersion = _version.Version.ToString();
                    task.AddOrUpdate("Project/PropertyGroup/AssemblyVersion", newVersion);
                    task.AddOrUpdate("Project/PropertyGroup/FileVersion", newVersion);
                    task.Execute(context);
                }
            }

            return res;
        }
    }
}