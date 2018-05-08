using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlubuCore.Context;
using FlubuCore.Scripting;
using FlubuCore.Tasks;

namespace FlubuCore.Targeting
{
    public class Target : TaskBase<int, Target>, ITarget
    {
        private readonly Dictionary<string, TaskExecutionMode> _dependencies = new Dictionary<string, TaskExecutionMode>();

        private readonly List<TaskGroup> _taskGroups = new List<TaskGroup>();

        private readonly CommandArguments _args;

        private readonly TargetTree _targetTree;

        internal Target(TargetTree targetTree, string targetName, CommandArguments args)
        {
            _targetTree = targetTree;
            TargetName = targetName;
            _args = args;
        }

        public Dictionary<string, TaskExecutionMode> Dependencies => _dependencies;

        public List<TaskGroup> TasksGroups => _taskGroups;

        /// <summary>
        ///     Gets a value indicating whether this target is hidden. Hidden targets will not be
        ///     visible in the list of targets displayed to the user as help.
        /// </summary>
        /// <value><c>true</c> if this target is hidden; otherwise, <c>false</c>.</value>
        public bool IsHidden { get; private set; }

        public string TargetName { get; }

        protected override bool LogDuration => true;

        protected override string Description { get; set; }

        string ITarget.Description
        {
            get { return Description; }
        }

        protected override string DescriptionForLog => TargetName;

        /// <summary>
        ///     Specifies targets on which this target depends on.
        /// </summary>
        /// <param name="targetNames">The dependency target names.</param>
        /// <returns>This same instance of <see cref="ITarget" />.</returns>
        public ITarget DependsOn(params string[] targetNames)
        {
            foreach (string dependentTargetName in targetNames)
            {
                _dependencies.Add(dependentTargetName, TaskExecutionMode.Synchronous);
            }

            return this;
        }

        /// <summary>
        ///     Specifies targets on which this target depends on.
        /// </summary>
        /// <param name="targetNames">The dependency target names.</param>
        /// <returns>This same instance of <see cref="ITarget" />.</returns>
        public ITarget DependsOnAsync(params string[] targetNames)
        {
            foreach (string dependentTargetName in targetNames)
            {
                _dependencies.Add(dependentTargetName, TaskExecutionMode.Parallel);
            }

            return this;
        }

        public ITarget DoAsync(Action<ITaskContextInternal> targetAction, Action<DoTask> taskAction = null, TaskGroup taskGroup = null)
        {
            var task = new DoTask(targetAction);
            taskAction?.Invoke(task);
            AddTaskToTaskGroup(taskGroup, task, TaskExecutionMode.Parallel);

            return this;
        }

        public ITarget DoAsync<T>(Action<ITaskContextInternal, T> targetAction, T param, Action<DoTask2<T>> taskAction = null, TaskGroup taskGroup = null)
        {
            var task = new DoTask2<T>(targetAction, param);
            taskAction?.Invoke(task);
            AddTaskToTaskGroup(taskGroup, task, TaskExecutionMode.Parallel);
            return this;
        }

        public ITarget DoAsync<T, T2>(Action<ITaskContextInternal, T, T2> targetAction, T param, T2 param2, Action<DoTask3<T, T2>> taskAction = null, TaskGroup taskGroup = null)
        {
            var task = new DoTask3<T, T2>(targetAction, param, param2);
            taskAction?.Invoke(task);
            AddTaskToTaskGroup(taskGroup, task, TaskExecutionMode.Parallel);
            return this;
        }

        public ITarget DoAsync<T1, T2, T3>(Action<ITaskContextInternal, T1, T2, T3> targetAction, T1 param, T2 param2, T3 param3, Action<DoTask4<T1, T2, T3>> taskAction = null, TaskGroup taskGroup = null)
        {
            var task = new DoTask4<T1, T2, T3>(targetAction, param, param2, param3);
            taskAction?.Invoke(task);
            AddTaskToTaskGroup(taskGroup, task, TaskExecutionMode.Parallel);
            return this;
        }

        public ITarget DoAsync<T1, T2, T3, T4>(Action<ITaskContextInternal, T1, T2, T3, T4> targetAction, T1 param, T2 param2, T3 param3, T4 param4, Action<DoTask5<T1, T2, T3, T4>> taskAction = null, TaskGroup taskGroup = null)
        {
            var task = new DoTask5<T1, T2, T3, T4>(targetAction, param, param2, param3, param4);
            taskAction?.Invoke(task);
            AddTaskToTaskGroup(taskGroup, task, TaskExecutionMode.Parallel);
            return this;
        }

        public ITarget DoAsync<T1, T2, T3, T4, T5>(Action<ITaskContextInternal, T1, T2, T3, T4, T5> targetAction, T1 param, T2 param2, T3 param3, T4 param4, T5 param5, Action<DoTask6<T1, T2, T3, T4, T5>> taskAction = null, TaskGroup taskGroup = null)
        {
            var task = new DoTask6<T1, T2, T3, T4, T5>(targetAction, param, param2, param3, param4, param5);
            taskAction?.Invoke(task);
            AddTaskToTaskGroup(taskGroup, task, TaskExecutionMode.Parallel);
            return this;
        }

        public ITarget DoAsync(Func<ITaskContextInternal, Task> targetAction, Action<DoTaskAsync> taskAction = null, TaskGroup taskGroup = null)
        {
            var task = new DoTaskAsync(targetAction);
            taskAction?.Invoke(task);
            AddTaskToTaskGroup(taskGroup, task, TaskExecutionMode.Parallel);
            return this;
        }

        public ITarget DoAsync<T>(Func<ITaskContextInternal, T, Task> targetAction, T param, Action<DoTaskAsync2<T>> taskAction = null, TaskGroup taskGroup = null)
        {
            var task = new DoTaskAsync2<T>(targetAction, param);
            taskAction?.Invoke(task);
            AddTaskToTaskGroup(taskGroup, task, TaskExecutionMode.Parallel);
            return this;
        }

        public ITarget DoAsync<T, T2>(Func<ITaskContextInternal, T, T2, Task> targetAction, T param, T2 param2, Action<DoTaskAsync3<T, T2>> taskAction = null, TaskGroup taskGroup = null)
        {
            var task = new DoTaskAsync3<T, T2>(targetAction, param, param2);
            taskAction?.Invoke(task);
            AddTaskToTaskGroup(taskGroup, task, TaskExecutionMode.Parallel);
            return this;
        }

        public ITarget DoAsync<T1, T2, T3>(Func<ITaskContextInternal, T1, T2, T3, Task> targetAction, T1 param, T2 param2, T3 param3, Action<DoTaskAsync4<T1, T2, T3>> taskAction = null, TaskGroup taskGroup = null)
        {
            var task = new DoTaskAsync4<T1, T2, T3>(targetAction, param, param2, param3);
            taskAction?.Invoke(task);
            AddTaskToTaskGroup(taskGroup, task, TaskExecutionMode.Parallel);
            return this;
        }

        public ITarget DoAsync<T1, T2, T3, T4>(Func<ITaskContextInternal, T1, T2, T3, T4, Task> targetAction, T1 param, T2 param2, T3 param3, T4 param4, Action<DoTaskAsync5<T1, T2, T3, T4>> taskAction = null, TaskGroup taskGroup = null)
        {
            var task = new DoTaskAsync5<T1, T2, T3, T4>(targetAction, param, param2, param3, param4);
            taskAction?.Invoke(task);
            AddTaskToTaskGroup(taskGroup, task, TaskExecutionMode.Parallel);
            return this;
        }

        public ITarget DoAsync<T1, T2, T3, T4, T5>(Func<ITaskContextInternal, T1, T2, T3, T4, T5, Task> targetAction, T1 param, T2 param2, T3 param3, T4 param4, T5 param5, Action<DoTaskAsync6<T1, T2, T3, T4, T5>> taskAction = null, TaskGroup taskGroup = null)
        {
            var task = new DoTaskAsync6<T1, T2, T3, T4, T5>(targetAction, param, param2, param3, param4, param5);
            taskAction?.Invoke(task);
            AddTaskToTaskGroup(taskGroup, task, TaskExecutionMode.Parallel);
            return this;
        }

        public ITarget Do(Action<ITaskContextInternal> targetAction, Action<DoTask> taskAction = null, TaskGroup taskGroup = null)
        {
            var task = new DoTask(targetAction);
            taskAction?.Invoke(task);
            AddTaskToTaskGroup(taskGroup, task, TaskExecutionMode.Synchronous);
            return this;
        }

        public ITarget Do<T>(Action<ITaskContextInternal, T> targetAction, T param, Action<DoTask2<T>> taskAction = null, TaskGroup taskGroup = null)
        {
            var task = new DoTask2<T>(targetAction, param);
            taskAction?.Invoke(task);
            AddTaskToTaskGroup(taskGroup, task, TaskExecutionMode.Synchronous);
            return this;
        }

        public ITarget Do<T, T2>(Action<ITaskContextInternal, T, T2> targetAction, T param, T2 param2, Action<DoTask3<T, T2>> taskAction = null, TaskGroup taskGroup = null)
        {
            var task = new DoTask3<T, T2>(targetAction, param, param2);
            taskAction?.Invoke(task);
            AddTaskToTaskGroup(taskGroup, task, TaskExecutionMode.Synchronous);
            return this;
        }

        public ITarget Do<T1, T2, T3>(Action<ITaskContextInternal, T1, T2, T3> targetAction, T1 param, T2 param2, T3 param3, Action<DoTask4<T1, T2, T3>> taskAction = null, TaskGroup taskGroup = null)
        {
            var task = new DoTask4<T1, T2, T3>(targetAction, param, param2, param3);
            taskAction?.Invoke(task);
            AddTaskToTaskGroup(taskGroup, task, TaskExecutionMode.Synchronous);
            return this;
        }

        public ITarget Do<T1, T2, T3, T4>(Action<ITaskContextInternal, T1, T2, T3, T4> targetAction, T1 param, T2 param2, T3 param3, T4 param4, Action<DoTask5<T1, T2, T3, T4>> taskAction = null, TaskGroup taskGroup = null)
        {
            var task = new DoTask5<T1, T2, T3, T4>(targetAction, param, param2, param3, param4);
            taskAction?.Invoke(task);
            AddTaskToTaskGroup(taskGroup, task, TaskExecutionMode.Synchronous);
            return this;
        }

        public ITarget Do<T1, T2, T3, T4, T5>(Action<ITaskContextInternal, T1, T2, T3, T4, T5> targetAction, T1 param, T2 param2, T3 param3, T4 param4, T5 param5, Action<DoTask6<T1, T2, T3, T4, T5>> taskAction = null, TaskGroup taskGroup = null)
        {
            var task = new DoTask6<T1, T2, T3, T4, T5>(targetAction, param, param2, param3, param4, param5);
            taskAction?.Invoke(task);
            AddTaskToTaskGroup(taskGroup, task, TaskExecutionMode.Synchronous);
            return this;
        }

        /// <summary>
        ///     Sets the target as the default target for the runner.
        /// </summary>
        /// <returns>This same instance of <see cref="ITarget" />.</returns>
        public ITarget SetAsDefault()
        {
            _targetTree.SetDefaultTarget(this);
            return this;
        }

        /// <summary>
        ///     Set's the description of the target,
        /// </summary>
        /// <param name="description">The description</param>
        /// <returns>this target</returns>
        public new ITarget SetDescription(string description)
        {
            Description = description;
            return this;
        }

        /// <summary>
        ///     Sets the target as hidden. Hidden targets will not be
        ///     visible in the list of targets displayed to the user as help.
        /// </summary>
        /// <returns>This same instance of <see cref="ITarget" />.</returns>
        public ITarget SetAsHidden()
        {
            IsHidden = true;
            return this;
        }

        /// <summary>
        ///     Specifies targets on which this target depends on.
        /// </summary>
        /// <param name="targets">The dependency targets</param>
        /// <returns>This same instance of <see cref="ITarget" /></returns>
        public ITarget DependsOn(params ITarget[] targets)
        {
            foreach (ITarget target in targets)
            {
                _dependencies.Add(target.TargetName, TaskExecutionMode.Synchronous);
            }

            return this;
        }

        public ITarget DependsOnAsync(params ITarget[] targets)
        {
            foreach (ITarget target in targets)
            {
                _dependencies.Add(target.TargetName, TaskExecutionMode.Parallel);
            }

            return this;
        }

        public ITarget AddTask(TaskGroup taskGroup, params ITask[] tasks)
        {
            foreach (var task in tasks)
            {
                AddTaskToTaskGroup(taskGroup, task, TaskExecutionMode.Synchronous);
            }

            return this;
        }

        public ITarget AddTaskAsync(TaskGroup taskGroup, params ITask[] tasks)
        {
            foreach (var task in tasks)
            {
                AddTaskToTaskGroup(taskGroup, task, TaskExecutionMode.Parallel);
            }

            return this;
        }

        public void TargetHelp(ITaskContextInternal context)
        {
            _targetTree.MarkTargetAsExecuted(this);
            context.LogInfo(" ");
            context.LogInfo($"Target {TargetName} will execute next tasks:");

            for (int i = 0; i < _taskGroups.Count; i++)
            {
                for (int j = 0; j < _taskGroups[i].Tasks.Count; j++)
                {
                    var task = (TaskHelp)_taskGroups[i].Tasks[j].task;
                    task.LogTaskHelp(context);
                }
            }
        }

        protected override int DoExecute(ITaskContextInternal context)
        {
            if (_targetTree == null)
            {
                throw new ArgumentNullException(nameof(_targetTree), "TargetTree must be set before Execution of target.");
            }

            context.LogInfo($"Executing target {TargetName}");

            _targetTree.MarkTargetAsExecuted(this);
            _targetTree.EnsureDependenciesExecuted(context, TargetName);

            if (_args.TargetsToExecute != null)
            {
                if (!_args.TargetsToExecute.Contains(TargetName))
                {
                    throw new TaskExecutionException($"Target {TargetName} is not on the TargetsToExecute list", 3);
                }
            }

            int taskGroupsCount = _taskGroups.Count;
            List<System.Threading.Tasks.Task> tasks = new List<System.Threading.Tasks.Task>();
            for (int i = 0; i < taskGroupsCount; i++)
            {
                int tasksCount = _taskGroups[i].Tasks.Count;
                try
                {
                    for (int j = 0; j < tasksCount; j++)
                    {
                        context.LogInfo($"Executing task {_taskGroups[i].Tasks[j].task.GetType().Name}");
                        if (_taskGroups[i].Tasks[j].taskExecutionMode == TaskExecutionMode.Synchronous)
                        {
                            _taskGroups[i].Tasks[j].task.ExecuteVoid(context);
                        }
                        else
                        {
                            tasks.Add(_taskGroups[i].Tasks[j].task.ExecuteVoidAsync(context));
                            if (j + 1 < tasksCount)
                            {
                                if (_taskGroups[i].Tasks[j + 1].taskExecutionMode != TaskExecutionMode.Synchronous)
                                    continue;
                                if (tasks.Count <= 0) continue;
                                Task.WaitAll(tasks.ToArray());
                                tasks = new List<Task>();
                            }
                            else if (i + 1 < taskGroupsCount)
                            {
                                if (_taskGroups[i + 1].Tasks[0].taskExecutionMode != TaskExecutionMode.Synchronous)
                                    continue;
                                if (tasks.Count <= 0) continue;
                                Task.WaitAll(tasks.ToArray());
                                tasks = new List<Task>();
                            }
                            else if (tasksCount > 0)
                            {
                                Task.WaitAll(tasks.ToArray());
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    _taskGroups[i].OnErrorAction?.Invoke(context, e);
                    throw;
                }
                finally
                {
                    _taskGroups[i].FinallyAction?.Invoke(context);
                }
            }

            return 0;
        }

        private void AddTaskToTaskGroup(TaskGroup taskGroup, ITask task, TaskExecutionMode taskExecutionMode)
        {
            if (taskGroup == null)
            {
                taskGroup = new TaskGroup()
                {
                    GroupId = Guid.NewGuid().ToString(),
                };

                taskGroup.Tasks.Add((task, taskExecutionMode));
                TasksGroups.Add(taskGroup);
            }
            else
            {
                var existingGroup = TasksGroups.FirstOrDefault(x => x.GroupId == taskGroup.GroupId);
                if (existingGroup == null)
                {
                    taskGroup.Tasks.Add((task, taskExecutionMode));
                    TasksGroups.Add(taskGroup);
                }
                else
                {
                    taskGroup.Tasks.Add((task, taskExecutionMode));
                }
            }
        }
    }
}