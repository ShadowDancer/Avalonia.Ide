using XamlX.TypeSystem;

namespace Avalonia.Ide.CompletionEngine.AssemblyMetadata
{
    public interface IFieldInformation : IXamlField
    {
        string ReturnTypeFullName { get; }
        bool IsRoutedEvent { get; }
    }
}
