using Avalonia.Ide.CompletionEngine.AssemblyMetadata;
using dnlib.DotNet;

namespace Avalonia.Ide.CompletionEngine.DnlibMetadataProvider
{
    class FieldWrapper : IFieldInformation
    {
        public FieldWrapper(FieldDef f)
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
        }

        public bool IsRoutedEvent { get; }

        public bool IsStatic { get; }

        public bool IsPublic { get; }

        public string Name { get; }

        public string ReturnTypeFullName { get; }
    }
}
