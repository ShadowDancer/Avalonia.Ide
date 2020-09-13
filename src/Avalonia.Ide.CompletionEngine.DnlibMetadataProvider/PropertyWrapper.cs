using System.Collections.Generic;
using System.Linq;
using Avalonia.Ide.CompletionEngine.AssemblyMetadata;
using dnlib.DotNet;
using XamlX.TypeSystem;

namespace Avalonia.Ide.CompletionEngine.DnlibMetadataProvider
{
    class PropertyWrapper : IPropertyInformation
    {
        public PropertyWrapper(PropertyDef prop, IInitContext ctx)
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

            CustomAttributes = prop.CustomAttributes.Select(n => new CustomAttributeWrapper(n, ctx)).ToList();


            if(setMethod != null)
            {
                Setter = new MethodWrapper(prop.SetMethod, ctx);
                PropertyType = ctx.GetTypeDef(prop.SetMethod.ReturnType.FullName);
            }
            
            if(getMethod != null)
            {
                Getter = new MethodWrapper(prop.GetMethod, ctx);
                if(PropertyType == null)
                {
                    PropertyType = ctx.GetTypeDef(prop.GetMethod.ReturnType.FullName);
                }
            }
        }

        public bool IsStatic { get; }
        public bool HasPublicSetter { get; }
        public bool HasPublicGetter { get; }
        public string TypeFullName { get; }
        public string Name { get; }

        public IXamlType PropertyType { get; }

        public IXamlMethod Setter { get; }

        public IXamlMethod Getter { get; }

        public IReadOnlyList<IXamlCustomAttribute> CustomAttributes { get; }

        public IReadOnlyList<IXamlType> IndexerParameters => throw new System.NotImplementedException();

        public bool Equals(IXamlProperty other)
        {
            throw new System.NotImplementedException();
        }

        public override string ToString() => Name;
    }
}
