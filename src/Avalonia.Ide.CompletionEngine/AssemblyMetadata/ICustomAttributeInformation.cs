using System.Collections.Generic;
using XamlX.TypeSystem;

namespace Avalonia.Ide.CompletionEngine.AssemblyMetadata
{
    public interface ICustomAttributeInformation : IXamlCustomAttribute
    {
        string TypeFullName { get; }
        IList<IAttributeConstructorArgumentInformation> ConstructorArguments { get; }
    }
}
