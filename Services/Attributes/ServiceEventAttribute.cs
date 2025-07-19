using System;

namespace YetAnotherMonitor.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ServiceEventAttribute : Attribute
    {
        public enum ServiceEventTypes
        {
            Updated = 0,
            Error,
            Started
        }
        public Type ServiceType { get; }
        public ServiceEventTypes ServiceEventType { get; }
        public ServiceEventAttribute(Type serviceType, ServiceEventTypes serviceEventType)
        {
            this.ServiceType = serviceType;
            this.ServiceEventType = serviceEventType;
        }
    }
}
