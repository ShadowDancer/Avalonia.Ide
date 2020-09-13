﻿using System;
using System.Collections.Generic;
using System.Text;
using XamlX.TypeSystem;

namespace Avalonia.Ide.CompletionEngine.AssemblyMetadata
{
    public interface IMetadataProvider
    {
        IMetadataReaderSession GetMetadata(IEnumerable<string> paths);
    }

    public interface IMetadataReaderSession : IDisposable
    {
        IEnumerable<IAssemblyInformation> Assemblies { get; }

        IXamlTypeSystem GetTypeSystem();
    }
}
