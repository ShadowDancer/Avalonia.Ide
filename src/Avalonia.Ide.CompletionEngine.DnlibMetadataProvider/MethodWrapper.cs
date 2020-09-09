using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Ide.CompletionEngine.AssemblyMetadata;
using dnlib.DotNet;

namespace Avalonia.Ide.CompletionEngine.DnlibMetadataProvider
{
    class MethodWrapper : IMethodInformation
    {
        private readonly MethodDef _method;
        private readonly Lazy<IList<IParameterInformation>> _parameters;

        public MethodWrapper(MethodDef method)
        {
            _method = method;
            _parameters = new Lazy<IList<IParameterInformation>>(() =>
                _method.Parameters.Skip(_method.IsStatic ? 0 : 1).Select(p => (IParameterInformation)new ParameterWrapper(p)).ToList() as
                    IList<IParameterInformation>);
        }

        public bool IsStatic => _method.IsStatic;
        public bool IsPublic => _method.IsPublic;
        public string Name => _method.Name;
        public IList<IParameterInformation> Parameters => _parameters.Value;
        public string ReturnTypeFullName => _method.ReturnType?.FullName;
        public override string ToString() => Name;
    }
}
