using Avalonia.Ide.CompletionEngine.AssemblyMetadata;
using dnlib.DotNet;

namespace Avalonia.Ide.CompletionEngine.DnlibMetadataProvider
{
    class PropertyWrapper : IPropertyInformation
    {
        public PropertyWrapper(PropertyDef prop)
        {
            Name = prop.Name;
            var setMethod = prop.SetMethod;
            var getMethod = prop.GetMethod;

            IsStatic = setMethod?.IsStatic ?? getMethod?.IsStatic ?? false;

            if (setMethod?.IsPublicOrInternal() == true)
            {
                HasPublicSetter = true;
                TypeFullName = setMethod.Parameters[setMethod.IsStatic ? 0 : 1].Type.FullName;
            }

            if (getMethod?.IsPublicOrInternal() == true)
            {
                HasPublicGetter = true;
                if (TypeFullName == null)
                    TypeFullName = getMethod.ReturnType.FullName;
            }
        }

        public bool IsStatic { get; }
        public bool HasPublicSetter { get; }
        public bool HasPublicGetter { get; }
        public string TypeFullName { get; }
        public string Name { get; }
        public override string ToString() => Name;
    }
}
