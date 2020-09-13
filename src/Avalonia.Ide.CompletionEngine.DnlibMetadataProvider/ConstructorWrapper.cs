using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using dnlib.DotNet;
using XamlX.TypeSystem;

namespace Avalonia.Ide.CompletionEngine.DnlibMetadataProvider
{
    class ConstructorWrapper : IXamlConstructor
    {
        public ConstructorWrapper(MethodDef constructor, IInitContext types)
        {
            IsPublic = constructor.IsPublic;
            IsStatic = constructor.IsStatic;
            Parameters = constructor.Parameters.Select(m => types.GetTypeDef(m.Type.FullName)).ToList();
        }

        public bool IsPublic { get; }

        public bool IsStatic { get; }

        public IReadOnlyList<IXamlType> Parameters { get; }

        public bool Equals(IXamlConstructor other)
        {
            throw new NotImplementedException();
        }
    }
}
