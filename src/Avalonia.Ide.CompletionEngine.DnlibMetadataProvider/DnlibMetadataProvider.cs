using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Ide.CompletionEngine.AssemblyMetadata;
using dnlib.DotNet;
using XamlX.TypeSystem;

namespace Avalonia.Ide.CompletionEngine.DnlibMetadataProvider
{
    public class DnlibMetadataProvider : IMetadataProvider
    {
        
        public IMetadataReaderSession GetMetadata(IEnumerable<string> paths)
        {
            return new DnlibMetadataProviderSession(paths.ToArray());
        }
    }

    class DnlibMetadataProviderSession :IMetadataReaderSession
    { 
        public IEnumerable<IAssemblyInformation> Assemblies { get; }

        Dictionary<string, TypeWrapper> Types { get; } = new Dictionary<string, TypeWrapper>();

        public IXamlTypeSystem GetTypeSystem()
        {
            return new DnlibTypeSystem(Assemblies.ToList());
        }
        
        public DnlibMetadataProviderSession(string[] directoryPath)
        {
            var assemblyDefs = LoadAssemblies(directoryPath)
                .ToList();

            Dictionary<string, (TypeDef, TypeWrapper)> types = new Dictionary<string, (TypeDef, TypeWrapper)>();
            foreach (var assembly in assemblyDefs)
            {
                foreach(var typeDef in assembly.Modules.SelectMany(m => m.Types))
                {
                    if(typeDef.FullName == "<Module>")
                    {
                        continue;
                    }
                    if (!types.ContainsKey(typeDef.FullName))
                    {
                        types.Add(typeDef.FullName, (typeDef, TypeWrapper.FromDef(typeDef)));
                    }                    
                }
            }

            Types = types.ToDictionary(n => n.Key, n => n.Value.Item2);
            Dictionary<string, AssemblyWrapper> assemblies = new Dictionary<string, AssemblyWrapper>();

            var ctx = new InitContext(Types);
            Assemblies = assemblyDefs.Select(n =>
            {
                var wrapper = new AssemblyWrapper(n, ctx);
                assemblies.Add(wrapper.FullName, wrapper);
                return wrapper;
            }).ToList();

            foreach ((TypeDef def, TypeWrapper wrapper) in types.Values)
            {
                wrapper.Initialize(def, ctx, assemblies[def.DefinitionAssembly.FullName]);
            }
        }

        static List<AssemblyDef> LoadAssemblies(string[] lst)
        {
            AssemblyResolver asmResolver = new AssemblyResolver();
            ModuleContext modCtx = new ModuleContext(asmResolver);
            asmResolver.DefaultModuleContext = modCtx;
            asmResolver.EnableTypeDefCache = true;

            foreach (var path in lst)
                asmResolver.PreSearchPaths.Add(path);

            List<AssemblyDef> assemblies = new List<AssemblyDef>();

            foreach (var asm in lst)
            {
                try
                {
                    var def = AssemblyDef.Load(File.ReadAllBytes(asm));
                    def.Modules[0].Context = modCtx;
                    asmResolver.AddToCache(def);
                    assemblies.Add(def);
                }
                catch
                {
                    //Ignore
                }
            }

            return assemblies;
        }

        public void Dispose()
        {
            //no-op
        }
    }

    internal class MetadataContext
    {
        Dictionary<string, AssemblyWrapper> Assemblies { get; } = new Dictionary<string, AssemblyWrapper>();

    } 
}
