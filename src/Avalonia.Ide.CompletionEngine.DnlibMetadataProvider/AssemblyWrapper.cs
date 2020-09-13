using System.Collections.Generic;
using System.IO;
using System.Linq;
using Avalonia.Ide.CompletionEngine.AssemblyMetadata;
using dnlib.DotNet;
using XamlX.TypeSystem;

namespace Avalonia.Ide.CompletionEngine.DnlibMetadataProvider
{
    class AssemblyWrapper : IAssemblyInformation, IXamlAssembly
    {
        private readonly AssemblyDef _asm;
        private readonly List<TypeWrapper> _types;
        private readonly List<CustomAttributeWrapper> _customAttributes;

        public AssemblyWrapper(AssemblyDef asm, IInitContext ctx)
        {
            _asm = asm;
            _types = _asm.Modules.SelectMany(m => m.Types)
                .Select(n => ctx.GetTypeDef(n.FullName))
                .Where(n => n != null)
                .ToList();
            _customAttributes = _asm.CustomAttributes.Select(a => new CustomAttributeWrapper(a, ctx)).ToList();
        }

        public string Name => _asm.Name;

        public string FullName => _asm.FullName;

        public IEnumerable<ITypeInformation> Types => _types;

        public IEnumerable<ICustomAttributeInformation> CustomAttributes => _customAttributes;
        public IEnumerable<string> ManifestResourceNames
            => _asm.ManifestModule.Resources.Select(r => r.Name.ToString()).ToList();

        IReadOnlyList<IXamlCustomAttribute> IXamlAssembly.CustomAttributes => _customAttributes;

        public bool Equals(IXamlAssembly other)
        {
            return _asm.FullName == ((AssemblyWrapper)other)._asm.FullName;
        }

        public IXamlType FindType(string fullName)
        {
            return Types.Where(n => n.FullName == fullName).FirstOrDefault();
        }

        public Stream GetManifestResourceStream(string name)
            => _asm.ManifestModule.Resources.FindEmbeddedResource(name).CreateReader().AsStream();

        public override string ToString() => Name;
    }
}
