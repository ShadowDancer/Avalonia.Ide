using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Ide.CompletionEngine.AssemblyMetadata;
using dnlib.DotNet;

namespace Avalonia.Ide.CompletionEngine.DnlibMetadataProvider
{
    class TypeWrapper : ITypeInformation
    {
        private readonly TypeDef _type;

        public static TypeWrapper FromDef(TypeDef def) => def == null ? null : new TypeWrapper(def);

        TypeWrapper(TypeDef type)
        {
            if (type == null)
                throw new ArgumentNullException();
            _type = type;
        }

        public string FullName => _type.FullName;
        public string Name => _type.Name;
        public string Namespace => _type.Namespace;
        public ITypeInformation GetBaseType() => FromDef(_type.GetBaseType().ResolveTypeDef());


        public IEnumerable<IEventInformation> Events => _type.Events.Select(e => new EventWrapper(e));

        public IEnumerable<IMethodInformation> Methods => _type.Methods.Select(m => new MethodWrapper(m));

        public IEnumerable<IFieldInformation> Fields => _type.Fields.Select(f => new FieldWrapper(f));

        public IEnumerable<IPropertyInformation> Properties => _type.Properties
            //Filter indexer properties
            .Where(p =>
                (p.GetMethod?.IsPublicOrInternal() == true && p.GetMethod.Parameters.Count == (p.GetMethod.IsStatic ? 0 : 1))
                || (p.SetMethod?.IsPublicOrInternal() == true && p.SetMethod.Parameters.Count == (p.SetMethod.IsStatic ? 1 : 2)))
            // Filter property overrides
            .Where(p => !p.Name.Contains("."))
            .Select(p => new PropertyWrapper(p));
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
        public IEnumerable<ITypeInformation> NestedTypes =>
            _type.HasNestedTypes ? _type.NestedTypes.Select(t => new TypeWrapper(t)) : Array.Empty<TypeWrapper>();
    }
}
