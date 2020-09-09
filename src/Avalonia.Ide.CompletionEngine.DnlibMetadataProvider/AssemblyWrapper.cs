using System.Collections.Generic;
using System.IO;
using System.Linq;
using Avalonia.Ide.CompletionEngine.AssemblyMetadata;
using dnlib.DotNet;

namespace Avalonia.Ide.CompletionEngine.DnlibMetadataProvider
{
    class AssemblyWrapper : IAssemblyInformation
    {
        private readonly AssemblyDef _asm;

        public AssemblyWrapper(AssemblyDef asm)
        {
            _asm = asm;
        }

        public string Name => _asm.Name;

        public IEnumerable<ITypeInformation> Types
            => _asm.Modules.SelectMany(m => m.Types).Select(TypeWrapper.FromDef);

        public IEnumerable<ICustomAttributeInformation> CustomAttributes
            => _asm.CustomAttributes.Select(a => new CustomAttributeWrapper(a));

        public IEnumerable<string> ManifestResourceNames
            => _asm.ManifestModule.Resources.Select(r => r.Name.ToString());

        public Stream GetManifestResourceStream(string name)
            => _asm.ManifestModule.Resources.FindEmbeddedResource(name).CreateReader().AsStream();

        public override string ToString() => Name;
    }
}
