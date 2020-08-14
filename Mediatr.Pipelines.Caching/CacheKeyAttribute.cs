using System;
using System.Collections.Generic;
using System.Text;

namespace Mediatr.Pipelines.Caching
{
    [AttributeUsage(AttributeTargets.Property)]
    public class CacheKeyAttribute: Attribute
    {
    }
}
