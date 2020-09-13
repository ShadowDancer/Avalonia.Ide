using System;
using System.Collections.Generic;
using System.Text;

namespace Avalonia.Ide.CompletionEngine.DnlibMetadataProvider
{
    public class InitContext : IInitContext
    {
        private readonly Dictionary<string, TypeWrapper> _types;

        internal InitContext(Dictionary<string, TypeWrapper> types)
        {
            _types = types;
        }

        TypeWrapper IInitContext.GetTypeDef(string fullName)
        {
            _types.TryGetValue(fullName, out var type);
            return type;
        }
    }
}
