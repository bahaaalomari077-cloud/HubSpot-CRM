using Crm.Application.Messaging.Providers;
using Crm.Domain.Enums;

namespace Crm.Infrastructure.Messaging;

/// <summary>
/// Resolves a provider by channel from all registered IMessageProvider instances.
/// Adding a new channel means registering one more provider — no change here.
/// </summary>
public class MessageProviderFactory : IMessageProviderFactory
{
    private readonly IReadOnlyDictionary<MessageChannel, IMessageProvider> _byChannel;

    public MessageProviderFactory(IEnumerable<IMessageProvider> providers)
    {
        // Last one registered per channel wins (allows overriding in tests).
        var map = new Dictionary<MessageChannel, IMessageProvider>();
        foreach (var p in providers)
            map[p.Channel] = p;
        _byChannel = map;
    }

    public bool Supports(MessageChannel channel) => _byChannel.ContainsKey(channel);

    public IMessageProvider For(MessageChannel channel) =>
        _byChannel.TryGetValue(channel, out var provider)
            ? provider
            : throw new InvalidOperationException($"No provider registered for channel '{channel}'.");
}
