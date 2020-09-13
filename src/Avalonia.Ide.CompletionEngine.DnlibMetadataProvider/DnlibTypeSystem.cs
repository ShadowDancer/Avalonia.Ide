using System;
using System.Collections.Generic;
using System.Linq;
using XamlX.TypeSystem;

namespace Avalonia.Ide.CompletionEngine.DnlibMetadataProvider
{
    public class DnlibTypeSystem : IXamlTypeSystem
    {
        private IReadOnlyList<IXamlAssembly> _assemblies;

        internal DnlibTypeSystem(IReadOnlyList<IXamlAssembly> assemblies)
        {
            _assemblies = assemblies;
        }

        public IReadOnlyList<IXamlAssembly> Assemblies => _assemblies;
        
        public IXamlAssembly FindAssembly(string name)
        {
            return FindAssemblyImpl(name);
        }

        private IXamlAssembly FindAssemblyImpl(string name)
        {
            return _assemblies.FirstOrDefault(a => a.Name.ToLowerInvariant() == name.ToLowerInvariant());
        }

        public IXamlType FindType(string name)
        {
            foreach (var asm in _assemblies)
            {
                var type = asm.FindType(name);
                if (type != null)
                {
                    return type;
                }
            }

            return null;
        }

        public IXamlType FindType(string name, string assembly)
        {
            var xamlAssembly = FindAssemblyImpl(assembly);
            var type = xamlAssembly.FindType(name);
            return type;
        }
    }
}