using System.Collections.Concurrent;
using System;
using System.Threading;
using System.Collections.Generic;

namespace HoweFramework
{

    public static class TypeId
    {
        internal static int GlobalCounter = 0;

        private static readonly ConcurrentDictionary<int, Type> _idToType = new();
        private static readonly ConcurrentDictionary<Type, int> _typeToId = new();

        /// <summary>
        /// Register a type and get the id.
        /// </summary>
        /// <param name="type">The type to register.</param>
        /// <returns>The id of the type.</returns>
        internal static int RegisterType(Type type)
        {
            if (_typeToId.TryGetValue(type, out var existing))
            {
                return existing;
            }

            var id = Interlocked.Increment(ref GlobalCounter);
            if (!_typeToId.TryAdd(type, id))
            {
                return _typeToId[type];
            }

            _idToType[id] = type;
            return id;
        }

        /// <summary>
        /// Get the type by the id.
        /// </summary>
        /// <param name="id">The id of the type.</param>
        /// <returns>The type.</returns>
        public static Type? GetTypeById(int id)
        {
            return _idToType.GetValueOrDefault(id);
        }

        /// <summary>
        /// Get the id of the type.
        /// </summary>
        /// <param name="type">The type to get the id of.</param>
        /// <returns>The id of the type.</returns>
        public static int GetIdByType(Type type)
        {
            if (_typeToId.TryGetValue(type, out var id))
            {
                return id;
            }

            // register the type.
            return RegisterType(type);
        }
    }

    /// <summary>
    /// Support unique type id in runtime.
    /// </summary>
    public static class TypeId<T>
    {
        public static readonly int Id = TypeId.RegisterType(typeof(T));
    }
}