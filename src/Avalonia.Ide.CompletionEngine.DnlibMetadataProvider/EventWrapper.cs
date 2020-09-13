using Avalonia.Ide.CompletionEngine.AssemblyMetadata;
using dnlib.DotNet;
using XamlX.TypeSystem;

namespace Avalonia.Ide.CompletionEngine.DnlibMetadataProvider
{
    class EventWrapper : IEventInformation, IXamlEventInfo
    {
        private readonly EventDef _event;

        public EventWrapper(EventDef @event)
        {
            _event = @event;
            Name = @event.Name;
            TypeFullName = @event.EventType.FullName;
        }

        public string Name { get; }

        public string TypeFullName { get; }

        public IXamlMethod Add => throw new System.NotImplementedException();

        public bool Equals(IXamlEventInfo other)
        {
            var otherWrapper = (EventWrapper)other;
            return _event.DeclaringType == otherWrapper._event.DeclaringType &&
                Name == otherWrapper.Name;
        }
    }
}
