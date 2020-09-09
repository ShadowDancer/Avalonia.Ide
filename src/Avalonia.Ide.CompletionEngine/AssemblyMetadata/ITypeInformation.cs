using System.Collections.Generic;
using XamlX.TypeSystem;

namespace Avalonia.Ide.CompletionEngine.AssemblyMetadata
{
    public interface ITypeInformation : IXamlType
    {
        ITypeInformation GetBaseType();
        new IEnumerable<IMethodInformation> Methods { get; }
        new IEnumerable<IPropertyInformation> Properties { get; }
        new IEnumerable<IEventInformation> Events { get; }
        new IEnumerable<IFieldInformation> Fields { get; }

        bool IsStatic { get; }
        bool IsPublic { get; }
        bool IsGeneric { get; }
        IEnumerable<string> EnumValues { get; }
        IEnumerable<ITypeInformation> NestedTypes { get; }
    }
}
