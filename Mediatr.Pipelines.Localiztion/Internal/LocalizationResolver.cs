using Microsoft.Extensions.Localization;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Mediatr.Pipelines.Localiztion
{

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

            foreach (var property in GetSetters(obj))
            {
                Resolve(property, obj);
            }
        }

        private IEnumerable<PropertyInfo> GetSetters(object obj) =>
            obj?.GetType().GetProperties().Where(p => p.CanWrite) ??
            new PropertyInfo[0];

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
