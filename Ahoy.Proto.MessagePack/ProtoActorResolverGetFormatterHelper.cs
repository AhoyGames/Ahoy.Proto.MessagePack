using Proto.Cluster;
using System;
using System.Collections.Generic;

namespace Proto.Serializer.MessagePack
{
    internal static class ProtoActorResolverGetFormatterHelper
    {
        private static readonly Dictionary<Type, object> FormatterMap = new Dictionary<Type, object>
        {
            {typeof(PID), new PIDFormatter()},
            {typeof(ClusterIdentity), new ClusterIdentityFormatter()},
        };

        internal static object GetFormatter(Type t) => FormatterMap.TryGetValue(t, out var formatter) ? formatter : null;
    }
}
