﻿using System.Collections.Generic;
using XamlX.TypeSystem;

namespace Avalonia.Ide.CompletionEngine.AssemblyMetadata
{
    public interface IMethodInformation : IXamlMethod
    {
        new IList<IParameterInformation> Parameters { get;}
        string ReturnTypeFullName { get; }
    }
}
