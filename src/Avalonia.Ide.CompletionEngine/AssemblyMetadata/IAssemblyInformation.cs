using System.Collections.Generic;
using System.IO;
using XamlX.TypeSystem;

namespace Avalonia.Ide.CompletionEngine.AssemblyMetadata
{
    public interface IAssemblyInformation : IXamlAssembly
    {
        IEnumerable<ITypeInformation> Types { get; }
        new IEnumerable<ICustomAttributeInformation> CustomAttributes { get; }
        IEnumerable<string> ManifestResourceNames { get; }
        Stream GetManifestResourceStream(string name);
    }
}
