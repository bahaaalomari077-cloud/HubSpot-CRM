using Crm.Domain.Enums;

namespace Crm.Application.Messaging.Providers;

/// <summary>Resolves the right provider for a channel.</summary>
public interface IMessageProviderFactory
{
    /// <summary>Returns the provider for the channel, or throws if none is registered.</summary>
    IMessageProvider For(MessageChannel channel);

    bool Supports(MessageChannel channel);
}
