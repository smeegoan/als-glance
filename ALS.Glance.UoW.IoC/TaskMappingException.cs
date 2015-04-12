using System;
using ALS.Glance.UoW.IoC.Properties;

namespace ALS.Glance.UoW.IoC
{
    public class TaskMappingException : Exception
    {
        private readonly Type _taskType;

        public TaskMappingException(Type taskType)
            : base(
                string.Format(
                    Resources.TaskMappingExceptionMessage,
                    taskType == null ? "<undefined>" : taskType.FullName))
        {
            _taskType = taskType;
        }

        public TaskMappingException(Type taskType, Exception e)
            : base(
                string.Format(
                    Resources.TaskMappingExceptionMessage,
                    taskType == null ? "<undefined>" : taskType.FullName), e)
        {
            _taskType = taskType;
        }

        public Type TaskType
        {
            get { return _taskType; }
        }
    }
}