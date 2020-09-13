namespace Avalonia.Ide.CompletionEngine.DnlibMetadataProvider
{
    interface IInitContext
    {
        TypeWrapper GetTypeDef(string fullName);
    }
}
