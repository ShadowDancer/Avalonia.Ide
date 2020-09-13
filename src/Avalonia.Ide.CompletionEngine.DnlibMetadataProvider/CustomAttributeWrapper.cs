using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Ide.CompletionEngine.AssemblyMetadata;
using dnlib.DotNet;
using XamlX.TypeSystem;

namespace Avalonia.Ide.CompletionEngine.DnlibMetadataProvider
{
    class CustomAttributeWrapper : ICustomAttributeInformation, IXamlCustomAttribute
    {
        private Lazy<IList<IAttributeConstructorArgumentInformation>> _args;
        public CustomAttributeWrapper(CustomAttribute attr, IInitContext ctx)
        {

            TypeFullName = attr.TypeFullName;
            _args = new Lazy<IList<IAttributeConstructorArgumentInformation>>(() =>
                attr.ConstructorArguments.Select(
                    ca => (IAttributeConstructorArgumentInformation)
                        new ConstructorArgumentWrapper(ca, ctx)).ToList());
            Type = ctx.GetTypeDef(attr.TypeFullName);
            Parameters = attr.ConstructorArguments.Select(ca =>
            {
                if(ca.Value is UTF8String)
                {
                    return ca.Value.ToString();
                }
                return ca.Value;
            }).ToList();
        }

        public string TypeFullName { get; }
        public IList<IAttributeConstructorArgumentInformation> ConstructorArguments => _args.Value;

        public IXamlType Type { get; }

        public List<object> Parameters { get; }

        public Dictionary<string, object> Properties => throw new NotImplementedException();

        public bool Equals(IXamlCustomAttribute other)
        {
            throw new NotImplementedException();
        }
    }
}
