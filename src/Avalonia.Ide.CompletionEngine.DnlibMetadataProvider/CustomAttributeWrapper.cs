using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Ide.CompletionEngine.AssemblyMetadata;
using dnlib.DotNet;

namespace Avalonia.Ide.CompletionEngine.DnlibMetadataProvider
{
    class CustomAttributeWrapper : ICustomAttributeInformation
    {
        private Lazy<IList<IAttributeConstructorArgumentInformation>> _args;
        public CustomAttributeWrapper(CustomAttribute attr)
        {
            TypeFullName = attr.TypeFullName;
            _args = new Lazy<IList<IAttributeConstructorArgumentInformation>>(() =>
                attr.ConstructorArguments.Select(
                    ca => (IAttributeConstructorArgumentInformation)
                        new ConstructorArgumentWrapper(ca)).ToList());
        }

        public string TypeFullName { get; }
        public IList<IAttributeConstructorArgumentInformation> ConstructorArguments => _args.Value;
    }
}
