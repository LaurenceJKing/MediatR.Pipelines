using MediatR;
using Microsoft.Extensions.Localization;
using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Mediatr.Pipelines.Localiztion
{
    public class LocalizeResponsePipeline<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly ILocalizationResolver localizationResolver;

        public LocalizeResponsePipeline(ILocalizationResolver localizationResolver)
        {
            this.localizationResolver = localizationResolver;
        }

        public async Task<TResponse> Handle(
            TRequest request,
            CancellationToken cancellationToken,
            RequestHandlerDelegate<TResponse> next)
        {
            var response = await next();
            localizationResolver.Localize(response);
            return response;
        }
    }


    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class LocalizeAttribute:Attribute
    {
        public string Key { get; set; }
    }

    public interface ILocalizationResolver
    {
        void Localize(object obj);
    }

    public class LocalizationResolver : ILocalizationResolver
    {
        private readonly IStringLocalizer localizer;

        public LocalizationResolver(IStringLocalizer localizer)
        {
            this.localizer = localizer;
        }

        public void Localize(object obj)
        {
            if (obj is null) return;

            foreach (var property in obj.GetType().GetProperties().Where(p => p.CanWrite))
            {
                Resolve(property, obj);
            }
        }

        private void Localize(PropertyInfo property, object obj)
        {
            if (!Localizable(property, out var attr)) return;

            var key = string.IsNullOrEmpty(attr.Key) ? property.Name : attr.Key;
            var localizedText = localizer[key];
            property.SetValue(obj, localizedText);
        }

        private void Resolve(PropertyInfo property, object obj)
        {
            if (property.PropertyType == typeof(string))
            {
                Localize(property, obj);
            }
            else if (IsEnumerable(property, obj, out var enumerable))
            {
                foreach (var instance in enumerable) Localize(instance);
            }
            else
            {
                Localize(property.GetValue(obj));
            }
        }

        private bool Localizable(PropertyInfo property, out LocalizeAttribute attribute)
        {
            attribute = null;
            if (property.CanWrite && property.PropertyType == typeof(string))
            {
                attribute = property.GetCustomAttributes<LocalizeAttribute>().SingleOrDefault();
            }
            return property != null;
        }

        private bool IsEnumerable(PropertyInfo property, object obj, out IEnumerable enumerable)
        {
            enumerable = null;
            switch (property.GetValue(obj))
            {
                case IEnumerable e: { enumerable = e; return e != null; }
                default: return false;
            }
        }
    }
}
