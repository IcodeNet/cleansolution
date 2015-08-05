// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StateTrackerSurrogateSelector.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.Extensions.Tracking
{
    using System;
    using System.CodeDom;
    using System.Collections.ObjectModel;
    using System.Reflection;
    using System.Runtime.Serialization;

    /// <summary>
    ///   Selects the correct surrogates for serializing
    /// </summary>
    internal class StateTrackerSurrogateSelector : IDataContractSurrogate
    {
        #region Public Methods and Operators

        /// <summary>
        /// During schema export operations, inserts annotations into the schema for non-null return values.
        /// </summary>
        /// <returns>
        /// An object that represents the annotation to be inserted into the XML schema definition. 
        /// </returns>
        /// <param name="memberInfo">
        /// A <see cref="T:System.Reflection.MemberInfo"/> that describes the member. 
        /// </param>
        /// <param name="dataContractType">
        /// A <see cref="T:System.Type"/> . 
        /// </param>
        /// <filterpriority>2</filterpriority>
        public object GetCustomDataToExport(MemberInfo memberInfo, Type dataContractType)
        {
            return null;
        }

        /// <summary>
        /// During schema export operations, inserts annotations into the schema for non-null return values.
        /// </summary>
        /// <returns>
        /// An object that represents the annotation to be inserted into the XML schema definition. 
        /// </returns>
        /// <param name="clrType">
        /// The CLR type to be replaced. 
        /// </param>
        /// <param name="dataContractType">
        /// The data contract type to be annotated. 
        /// </param>
        /// <filterpriority>2</filterpriority>
        public object GetCustomDataToExport(Type clrType, Type dataContractType)
        {
            return null;
        }

        /// <summary>
        /// During serialization, deserialization, and schema import and export, returns a data contract type that substitutes the specified type.
        /// </summary>
        /// <returns>
        /// The <see cref="T:System.Type"/> to substitute for the <paramref name="type"/> value. This type must be serializable by the <see cref="T:System.Runtime.Serialization.DataContractSerializer"/> . For example, it must be marked with the <see cref="T:System.Runtime.Serialization.DataContractAttribute"/> attribute or other mechanisms that the serializer recognizes. 
        /// </returns>
        /// <param name="type">
        /// The CLR type <see cref="T:System.Type"/> to substitute. 
        /// </param>
        public Type GetDataContractType(Type type)
        {
            if (type.IsAssignableFrom(typeof(StateTracker)))
            {
                return typeof(StateTrackerSurrogated);
            }

            if (type.IsAssignableFrom(typeof(StateMachineInfo)))
            {
                return typeof(StateMachineInfoSurrogated);
            }

            return type;
        }

        /// <summary>
        /// During deserialization, returns an object that is a substitute for the specified object.
        /// </summary>
        /// <returns>
        /// The substituted deserialized object. This object must be of a type that is serializable by the <see cref="T:System.Runtime.Serialization.DataContractSerializer"/> . For example, it must be marked with the <see cref="T:System.Runtime.Serialization.DataContractAttribute"/> attribute or other mechanisms that the serializer recognizes. 
        /// </returns>
        /// <param name="obj">
        /// The deserialized object to be substituted. 
        /// </param>
        /// <param name="targetType">
        /// The <see cref="T:System.Type"/> that the substituted object should be assigned to. 
        /// </param>
        public object GetDeserializedObject(object obj, Type targetType)
        {
            if (targetType.IsAssignableFrom(typeof(StateTracker)))
            {
                return new StateTracker(obj as StateTrackerSurrogated);
            }

            if (targetType.IsAssignableFrom(typeof(StateMachineInfo)))
            {
                return new StateMachineInfo(obj as StateMachineInfoSurrogated);
            }

            return obj;
        }

        /// <summary>
        /// Sets the collection of known types to use for serialization and deserialization of the custom data objects.
        /// </summary>
        /// <param name="customDataTypes">
        /// A <see cref="T:System.Collections.ObjectModel.Collection`1"/> of <see cref="T:System.Type"/> to add known types to. 
        /// </param>
        /// <filterpriority>2</filterpriority>
        public void GetKnownCustomDataTypes(Collection<Type> customDataTypes)
        {
        }

        /// <summary>
        /// During serialization, returns an object that substitutes the specified object.
        /// </summary>
        /// <returns>
        /// The substituted object that will be serialized. The object must be serializable by the <see cref="T:System.Runtime.Serialization.DataContractSerializer"/> . For example, it must be marked with the <see cref="T:System.Runtime.Serialization.DataContractAttribute"/> attribute or other mechanisms that the serializer recognizes. 
        /// </returns>
        /// <param name="obj">
        /// The object to substitute. 
        /// </param>
        /// <param name="targetType">
        /// The <see cref="T:System.Type"/> that the substituted object should be assigned to. 
        /// </param>
        public object GetObjectToSerialize(object obj, Type targetType)
        {
            var stateTracker = obj as StateTracker;
            if (stateTracker != null)
            {
                return new StateTrackerSurrogated(stateTracker);
            }

            var stateMachineInfo = obj as StateMachineInfo;
            if (stateMachineInfo != null)
            {
                return new StateMachineInfoSurrogated(stateMachineInfo);
            }

            return obj;
        }

        /// <summary>
        /// During schema import, returns the type referenced by the schema.
        /// </summary>
        /// <returns>
        /// The <see cref="T:System.Type"/> to use for the referenced type. 
        /// </returns>
        /// <param name="typeName">
        /// The name of the type in schema. 
        /// </param>
        /// <param name="typeNamespace">
        /// The namespace of the type in schema. 
        /// </param>
        /// <param name="customData">
        /// The object that represents the annotation inserted into the XML schema definition, which is data that can be used for finding the referenced type. 
        /// </param>
        /// <filterpriority>2</filterpriority>
        public Type GetReferencedTypeOnImport(string typeName, string typeNamespace, object customData)
        {
            return null;
        }

        /// <summary>
        /// Processes the type that has been generated from the imported schema.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.CodeDom.CodeTypeDeclaration"/> that contains the processed type. 
        /// </returns>
        /// <param name="typeDeclaration">
        /// A <see cref="T:System.CodeDom.CodeTypeDeclaration"/> to process that represents the type declaration generated during schema import. 
        /// </param>
        /// <param name="compileUnit">
        /// The <see cref="T:System.CodeDom.CodeCompileUnit"/> that contains the other code generated during schema import. 
        /// </param>
        /// <filterpriority>2</filterpriority>
        public CodeTypeDeclaration ProcessImportedType(CodeTypeDeclaration typeDeclaration, CodeCompileUnit compileUnit)
        {
            return null;
        }

        #endregion
    }
}