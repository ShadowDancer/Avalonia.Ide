using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Ide.CompletionEngine.AssemblyMetadata;
using dnlib.DotNet;
using XamlX.TypeSystem;

namespace Avalonia.Ide.CompletionEngine.DnlibMetadataProvider
{
    class MethodWrapper : IMethodInformation
    {
        private readonly MethodDef _method;
        private readonly Lazy<IList<IParameterInformation>> _parameters;
        private readonly IReadOnlyList<TypeWrapper> _xamlParameters;

        public MethodWrapper(MethodDef method, IInitContext ctx)
        {
            _method = method;
            _parameters = new Lazy<IList<IParameterInformation>>(() =>
                _method.Parameters.Skip(_method.IsStatic ? 0 : 1).Select(p => (IParameterInformation)new ParameterWrapper(p)).ToList());

            DeclaringType = ctx.GetTypeDef(_method.DeclaringType.FullName);
            ReturnType = ctx.GetTypeDef(_method.ReturnType.FullName);
            _xamlParameters = _method.Parameters.Select(n => ctx.GetTypeDef(n.Type.FullName)).ToList();
        }

        public bool IsStatic => _method.IsStatic;
        public bool IsPublic => _method.IsPublic;
        public string Name => _method.Name;
        public IList<IParameterInformation> Parameters => _parameters.Value;
        public string ReturnTypeFullName => _method.ReturnType?.FullName;

        public IXamlType ReturnType { get; }

        public IXamlType DeclaringType { get; }

        public IReadOnlyList<IXamlCustomAttribute> CustomAttributes { get; }

        IReadOnlyList<IXamlType> IXamlMethod.Parameters => _xamlParameters;

        public bool Equals(IXamlMethod other)
        {
            throw new NotImplementedException();
        }

        public IXamlMethod MakeGenericMethod(IReadOnlyList<IXamlType> typeArguments)
        {
            throw new NotImplementedException();
        }

        public override string ToString() => Name;
    }
}
