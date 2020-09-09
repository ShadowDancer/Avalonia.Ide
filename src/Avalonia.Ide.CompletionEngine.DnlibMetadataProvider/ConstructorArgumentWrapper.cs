using Avalonia.Ide.CompletionEngine.AssemblyMetadata;
using dnlib.DotNet;

namespace Avalonia.Ide.CompletionEngine.DnlibMetadataProvider
{
    class ConstructorArgumentWrapper : IAttributeConstructorArgumentInformation
    {
        public ConstructorArgumentWrapper(CAArgument ca)
        {
            Value = ca.Value;
        }

        public object Value { get; }
    }
}
