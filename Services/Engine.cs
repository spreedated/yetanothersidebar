using Services.Models.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Services
{
    public static class Engine
    {
        private static List<Service<IResponse>> _services;

        public static IEnumerable<Service<IResponse>> GetServices()
        {
            Assembly a = typeof(Engine).Assembly;

            var ty = a.GetTypes().Where(t => t.IsClass && !t.IsAbstract && t.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IService<>))).Select(t => t);

            foreach (Type s in ty)
            {
                Service<IResponse> service = (Service<IResponse>)Activator.CreateInstance(s);
                service.DataUpdated += (o, e) => { };
                _services.Add((Service<IResponse>)Activator.CreateInstance(s));
            }

            return null;
        }
    }
}
