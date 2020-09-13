using System.Collections.Generic;
using System.Linq;
using Avalonia.Ide.CompletionEngine.AssemblyMetadata;
using dnlib.DotNet;
using XamlX.TypeSystem;

namespace Avalonia.Ide.CompletionEngine.DnlibMetadataProvider
{
    class FieldWrapper : IFieldInformation, IXamlField
    {
        public FieldWrapper(FieldDef f, IInitContext ctx)
        {
            IsStatic = f.IsStatic;
            IsPublic = f.IsPublic || f.IsAssembly;
            Name = f.Name;
            ReturnTypeFullName = f.FieldType.FullName;

            bool isRoutedEvent = false;
            ITypeDefOrRef t = f.FieldType.ToTypeDefOrRef();
            while(t != null)
            {
                if(t.Name == "RoutedEvent" && t.Namespace == "Avalonia.Interactivity")
                {
                    isRoutedEvent = true;
                    break;
                }
                t = t.GetBaseType();
            }

            IsRoutedEvent = isRoutedEvent;
            FieldType = ctx.GetTypeDef(f.FieldType.FullName);
            IsLiteral = f.IsLiteral;
            CustomAttributes = f.CustomAttributes.Select(n => new CustomAttributeWrapper(n, ctx)).ToList();
        }

        public bool IsRoutedEvent { get; }

        public bool IsStatic { get; }

        public bool IsPublic { get; }

        public string Name { get; }

        public string ReturnTypeFullName { get; }

        public IXamlType FieldType { get; }

        public bool IsLiteral { get; }

        public IReadOnlyList<IXamlCustomAttribute> CustomAttributes { get; }

        public bool Equals(IXamlField other)
        {
            throw new System.NotImplementedException();
        }

        public object GetLiteralValue()
        {
            throw new System.NotImplementedException();
        }
    }
}
