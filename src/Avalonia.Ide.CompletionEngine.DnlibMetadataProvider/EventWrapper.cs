using Avalonia.Ide.CompletionEngine.AssemblyMetadata;
using dnlib.DotNet;

namespace Avalonia.Ide.CompletionEngine.DnlibMetadataProvider
{
    class EventWrapper : IEventInformation
    {
        public EventWrapper(EventDef @event)
        {
            Name = @event.Name;
            TypeFullName = @event.EventType.FullName;
        }

        public string Name { get; }

        public string TypeFullName { get; }
    }
}
