using XamlX.TypeSystem;

namespace Avalonia.Ide.CompletionEngine.AssemblyMetadata
{
    public interface IPropertyInformation : IXamlProperty
    {
        bool IsStatic { get; }
        bool HasPublicSetter { get; }
        bool HasPublicGetter { get; }
        string TypeFullName { get; }
        string Name { get; }
    }
}
