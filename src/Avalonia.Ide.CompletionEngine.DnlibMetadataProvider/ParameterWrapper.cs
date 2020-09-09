using Avalonia.Ide.CompletionEngine.AssemblyMetadata;
using dnlib.DotNet;

namespace Avalonia.Ide.CompletionEngine.DnlibMetadataProvider
{
    class ParameterWrapper : IParameterInformation
    {
        private readonly Parameter _param;

        public ParameterWrapper(Parameter param)
        {
            _param = param;
        }
        public string TypeFullName => _param.Type.FullName;
    }
}
