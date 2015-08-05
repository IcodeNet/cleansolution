// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReflectionClone.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.UnitTesting
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    ///   Clones an object using reflection
    /// </summary>
    public static class ReflectionClone
    {
        #region Public Methods and Operators

        /// <summary>
        /// Creates a deep copy.
        /// </summary>
        /// <param name="source">
        /// The source. 
        /// </param>
        /// <typeparam name="T">
        /// The type of the object 
        /// </typeparam>
        /// <returns>
        /// A deep copy of the object 
        /// </returns>
        public static T DeepCopy<T>(T source)
        {
            if (source.Equals(default(T)))
            {
                return source;
            }

            return (T)Clone(source);
        }

        #endregion

        #region Methods

        /// <summary>
        /// The clone.
        /// </summary>
        /// <param name="source">
        /// The source. 
        /// </param>
        /// <returns>
        /// A deep copy clone of the source 
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// Unable to clone the object
        /// </exception>
        private static object Clone(object source)
        {
            if (source == null)
            {
                return null;
            }

            var sourceType = source.GetType();

            if (sourceType.IsValueType || source is string)
            {
                return source;
            }

            if (sourceType.IsArray)
            {
                return CloneArray((Array)source, sourceType);
            }

            if (sourceType.IsClass)
            {
                return CloneClass(source, sourceType);
            }

            throw new InvalidOperationException("Unknown object type");
        }

        /// <summary>
        /// The clone array.
        /// </summary>
        /// <param name="array">
        /// The array. 
        /// </param>
        /// <param name="sourceType">
        /// The source type. 
        /// </param>
        /// <returns>
        /// A deep copy clone array. 
        /// </returns>
        private static object CloneArray(Array array, Type sourceType)
        {
            var elementType = sourceType.GetElementType();
            Debug.Assert(elementType != null, "elementType != null");
            var dest = Array.CreateInstance(elementType, array.Length);
            for (var i = 0; i < array.Length; i++)
            {
                dest.SetValue(Clone(array.GetValue(i)), i);
            }

            return dest;
        }

        /// <summary>
        /// The clone class.
        /// </summary>
        /// <param name="source">
        /// The source. 
        /// </param>
        /// <param name="sourceType">
        /// The source type. 
        /// </param>
        /// <returns>
        /// A deep copy clone class. 
        /// </returns>
        private static object CloneClass(object source, Type sourceType)
        {
            var dest = Activator.CreateInstance(sourceType);
            foreach (
                var fieldInfo in
                    from fieldInfo in
                        sourceType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                    let value = fieldInfo.GetValue(source)
                    where value != null
                    select fieldInfo)
            {
                fieldInfo.SetValue(dest, Clone(fieldInfo.GetValue(source)));
            }

            return dest;
        }

        #endregion
    }
}