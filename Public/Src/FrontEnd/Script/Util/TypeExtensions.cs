// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.ContractsLight;
using System.Reflection;
using BuildXL.FrontEnd.Script.Core;
using BuildXL.FrontEnd.Script.Literals;
using BuildXL.FrontEnd.Script.Types;
using BuildXL.FrontEnd.Script.Values;
using BuildXL.Utilities;
using BuildXL.Utilities.Configuration;

namespace BuildXL.FrontEnd.Script.Util
{
    /// <summary>
    /// Set of extension method for checking the runtime type of an object during evaluation/configuration processing.
    /// </summary>
    public static class TypeExtensions
    {
        private static readonly HashSet<RuntimeTypeHandle> s_primitiveTypes = new HashSet<RuntimeTypeHandle>(
            new[]
            {
                typeof(long).TypeHandle,
                typeof(ulong).TypeHandle,
                typeof(int).TypeHandle,
                typeof(uint).TypeHandle,
                typeof(short).TypeHandle,
                typeof(ushort).TypeHandle,
                typeof(sbyte).TypeHandle,
                typeof(byte).TypeHandle,
            });

        /// <nodoc />
        public static bool IsNullOrUndefined(object value)
        {
            return value == null || value == UndefinedValue.Instance || value == UndefinedLiteral.Instance;
        }

        /// <nodoc />
        public static bool IsNumberType(this RuntimeTypeHandle type)
        {
            return s_primitiveTypes.Contains(type);
        }

        /// <nodoc />
        public static bool IsUnitType(this RuntimeTypeHandle type)
        {
            return type.Equals(typeof(UnitValue).TypeHandle);
        }

        /// <nodoc />
        public static bool IsBooleanType(this RuntimeTypeHandle type)
        {
            return type.Equals(typeof(bool).TypeHandle);
        }

        /// <nodoc />
        public static bool IsEnumType(this RuntimeTypeHandle type)
        {
            return type.Equals(typeof(bool).TypeHandle);
        }

        /// <nodoc />
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
        public static bool IsPrimitiveOrEnum(this TypeInfo type)
        {
            return type.TypeHandle.IsNumberType() || type.TypeHandle.IsBooleanType() || type.IsEnum;
        }

        /// <nodoc />
        public static bool IsString(this RuntimeTypeHandle type)
        {
            return type.Equals(typeof(string).TypeHandle);
        }

        /// <nodoc />
        public static bool IsFullSymbol(this RuntimeTypeHandle type)
        {
            return type.Equals(typeof(FullSymbol).TypeHandle);
        }

        /// <nodoc />
        public static bool IsFileArtifact(this RuntimeTypeHandle type)
        {
            return type.Equals(typeof(FileArtifact).TypeHandle);
        }

        /// <nodoc />
        public static bool IsDirectoryArtifact(this RuntimeTypeHandle type)
        {
            return type.Equals(typeof(DirectoryArtifact).TypeHandle);
        }

        /// <nodoc />
        public static bool IsPathAtom(this RuntimeTypeHandle type)
        {
            return type.Equals(typeof(PathAtom).TypeHandle);
        }

        /// <nodoc />
        public static bool IsRelativePath(this RuntimeTypeHandle type)
        {
            return type.Equals(typeof(RelativePath).TypeHandle);
        }

        /// <nodoc />
        public static bool IsUnionType(this TypeInfo type)
        {
            return typeof(DiscriminatingUnion).IsAssignableFrom(type);
        }

        /// <nodoc />
        public static bool IsLazyEval(this TypeInfo type)
        {
            return typeof(ILazyEval).IsAssignableFrom(type);
        }

        /// <nodoc />
        public static bool IsAbsolutePath(this RuntimeTypeHandle type)
        {
            return type.Equals(typeof(AbsolutePath).TypeHandle);
        }

        /// <nodoc />
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
        public static bool IsListOfT(this TypeInfo type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition().TypeHandle.Equals(typeof(List<>).TypeHandle);
        }

        /// <nodoc />
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
        public static bool IsReadOnlyListOfT(this TypeInfo type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition().TypeHandle.Equals(typeof(IReadOnlyList<>).TypeHandle);
        }

        /// <nodoc />
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
        public static bool IsDictionaryOfT(this TypeInfo type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition().TypeHandle.Equals(typeof(Dictionary<,>).TypeHandle);
        }

        /// <nodoc />
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
        public static bool IsReadOnlyDictionaryOfT(this TypeInfo type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition().TypeHandle.Equals(typeof(IReadOnlyDictionary<,>).TypeHandle);
        }
    }
}
