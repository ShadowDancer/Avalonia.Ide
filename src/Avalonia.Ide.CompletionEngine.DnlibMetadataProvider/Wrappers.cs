using dnlib.DotNet;

namespace Avalonia.Ide.CompletionEngine.DnlibMetadataProvider
{
    static class WrapperExtensions
    {
        public static bool IsPublicOrInternal(this MethodDef methodDef)
                            => methodDef?.IsPublic == true || methodDef?.IsAssembly == true;
    }
}
