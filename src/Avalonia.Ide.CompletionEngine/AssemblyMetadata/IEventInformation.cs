using XamlX.TypeSystem;

namespace Avalonia.Ide.CompletionEngine.AssemblyMetadata
{
    public interface IEventInformation : IXamlEventInfo
    {
        string TypeFullName { get; }
    }
}
