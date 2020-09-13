using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Ide.CompletionEngine.AssemblyMetadata;
using dnlib.DotNet;
using XamlX.TypeSystem;

namespace Avalonia.Ide.CompletionEngine.DnlibMetadataProvider
{
    class TypeWrapper : ITypeInformation
    {
        private readonly TypeDef _type;
        private ITypeInformation _baseType;
        private List<EventWrapper> _events;
        private List<MethodWrapper> _methods;
        private List<FieldWrapper> _fields;
        private AssemblyWrapper _assembly;
        private List<PropertyWrapper> _properties;
        private List<CustomAttributeWrapper> _customAttributes;
        private List<ConstructorWrapper> _constructors;
        private List<TypeWrapper> _interfaces;

        public static TypeWrapper FromDef(TypeDef def) => def == null ? null : new TypeWrapper(def);

        TypeWrapper(TypeDef type)
        {
            if (type == null)
                throw new ArgumentNullException();
            FullName = type.FullName;
            Name = type.Name;
            Namespace = type.Namespace;
            _type = type;
            IsValueType = type.IsValueType;
        }

        public string FullName { get; }
        public string Name { get; }
        public string Namespace { get; }
        public ITypeInformation GetBaseType() => _baseType;


        public IEnumerable<IEventInformation> Events => _events;

        public IEnumerable<IMethodInformation> Methods => _methods;

        public IEnumerable<IFieldInformation> Fields => _fields;

        public IEnumerable<IPropertyInformation> Properties => _properties;

        internal void Initialize(TypeDef def, IInitContext ctx, AssemblyWrapper assembly)
        {
            if(def.BaseType != null)
            {
                _baseType = ctx.GetTypeDef(def.BaseType.FullName);
            }
            
            _events = _type.Events.Select(e => new EventWrapper(e)).ToList();
            _methods = _type.Methods.Select(m => new MethodWrapper(m, ctx)).ToList();
            _fields = _type.Fields.Select(f => new FieldWrapper(f, ctx)).ToList();
            _assembly = assembly;

            _properties =
                _type.Properties
                //Filter indexer properties
                .Where(p =>
                    (p.GetMethod?.IsPublicOrInternal() == true && p.GetMethod.Parameters.Count == (p.GetMethod.IsStatic ? 0 : 1))
                    || (p.SetMethod?.IsPublicOrInternal() == true && p.SetMethod.Parameters.Count == (p.SetMethod.IsStatic ? 1 : 2)))
                // Filter property overrides
                .Where(p => !p.Name.Contains("."))
                .Select(p => new PropertyWrapper(p, ctx)).ToList();

            GenericInstSig genericInstSig = _type.TryGetGenericInstSig();
            if (genericInstSig != null)
            {
                GenericTypeDefinition = ctx.GetTypeDef(genericInstSig.GenericType.FullName);
                GenericArguments = genericInstSig.GenericArguments.Select(n => ctx.GetTypeDef(n.FullName)).ToList();
            }

            SZArraySig arraySig = _type.TryGetSZArraySig();
            if (arraySig != null)
            {
                IsArray = true;
                if (GenericArguments.Count > 0)
                {
                    ArrayElementType = GenericArguments[0];
                }
                else
                {
                    //ArrayElementType = _type.Module.CorLibTypes.Object;
                }
            }
            else
            {
                IsArray = false;
                ArrayElementType = null;
            }

            _customAttributes = _type.CustomAttributes.Select(n => new CustomAttributeWrapper(n, ctx)).ToList();
            _constructors = _type.FindConstructors().Select(n => new ConstructorWrapper(n, ctx)).ToList();
            _interfaces = _type.Interfaces.Select(n => ctx.GetTypeDef(n.Interface.Namespace + "." + n.Interface.Name)).ToList();
        }

        public bool IsEnum => _type.IsEnum;
        public bool IsStatic => _type.IsAbstract && _type.IsSealed;
        public bool IsInterface => _type.IsInterface;
        public bool IsPublic => _type.IsPublic;
        public bool IsGeneric => _type.HasGenericParameters;
        public IEnumerable<string> EnumValues
        {
            get
            {
                return _type.Fields.Where(f => f.IsStatic).Select(f => f.Name.String).ToArray();
            }
        }
        public override string ToString() => Name;

        public bool IsAssignableFrom(IXamlType type)
        {
            if (!type.IsValueType
                && type == XamlPseudoType.Null)
                return true;

            if (type.IsValueType
                && GenericTypeDefinition?.FullName == "System.Nullable`1"
                && GenericArguments[0].Equals(type))
                return true;
            if (FullName == "System.Object" && type.IsInterface)
                return true;
            var baseType = type;
            while (baseType != null)
            {
                if (baseType.Equals(this))
                    return true;
                baseType = baseType.BaseType;
            }

            if (IsInterface && type.GetAllInterfaces().Any(IsAssignableFrom))
                return true;
            return false;
        }

        public IXamlType MakeGenericType(IReadOnlyList<IXamlType> typeArguments)
        {
            throw new NotImplementedException();
        }

        public IXamlType MakeArrayType(int dimensions)
        {
            throw new NotImplementedException();
        }

        public IXamlType GetEnumUnderlyingType()
        {
            throw new NotImplementedException();
        }

        public bool Equals(IXamlType other)
        {
            return Equals(this.FullName, other.FullName);
        }

        public IEnumerable<ITypeInformation> NestedTypes =>
            _type.HasNestedTypes ? _type.NestedTypes.Select(t => new TypeWrapper(t)) : Array.Empty<TypeWrapper>();

        public object Id => Guid.NewGuid();

        public IXamlAssembly Assembly => _assembly;

        IReadOnlyList<IXamlProperty> IXamlType.Properties => _properties;

        IReadOnlyList<IXamlEventInfo> IXamlType.Events => _events;

        IReadOnlyList<IXamlField> IXamlType.Fields => _fields;

        IReadOnlyList<IXamlMethod> IXamlType.Methods => _methods;

        public IReadOnlyList<IXamlConstructor> Constructors => _constructors;

        public IReadOnlyList<IXamlCustomAttribute> CustomAttributes => _customAttributes;

        public IReadOnlyList<IXamlType> GenericArguments { get; private set; }

        public IReadOnlyList<IXamlType> GenericParameters { get; } = new List<IXamlType>();

        public IXamlType GenericTypeDefinition { get; private set; }

        public bool IsArray { get; private set; }

        public IXamlType ArrayElementType { get; private set; }

        public IXamlType BaseType => _baseType;

        public bool IsValueType { get; }

        public IReadOnlyList<IXamlType> Interfaces => _interfaces;

    }
}
