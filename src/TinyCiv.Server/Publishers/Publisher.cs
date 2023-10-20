using System.Collections.Concurrent;
using Microsoft.AspNetCore.SignalR;
using TinyCiv.Server.Core.Extensions;
using TinyCiv.Server.Core.Publishers;
using TinyCiv.Shared.Events.Server;

namespace TinyCiv.Server.Publishers;

public class Publisher : IPublisher
{
    private readonly ConcurrentDictionary<string, Subscriber> _subscribers = new();

    private readonly ILogger<Publisher> _logger;

    public Publisher(ILogger<Publisher> logger)
    {
        _logger = logger;
    }
    
    public void Subscribe(Subscriber subscriber)
    {
        _subscribers.TryAdd(subscriber.ConnectionId ?? throw new InvalidOperationException(), subscriber);
    }

    public void Unsubscribe(Subscriber subscriber)
    {
        _subscribers.TryRemove(subscriber.ConnectionId ?? throw new InvalidOperationException(), out _);
    }

    public Task NotifyAllAsync<T>(string methodName, T serverEvent) where T : ServerEvent
    {
        var notificationTasks = new List<Task>();
        
        foreach (var subscriber in _subscribers.Values)
        {
            // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
            if (subscriber == null)
            {
                continue;
            }
            
            notificationTasks.Add(InternalNotifyAsync(subscriber.Proxy, methodName, serverEvent));
        }

        return Task.WhenAll(notificationTasks);
    }

    public Task NotifySubscriberAsync<T>(Subscriber subscriber, string methodName, T serverEvent) where T : ServerEvent
    {
        return InternalNotifyAsync(subscriber.Proxy, methodName, serverEvent);
    }
    
    private Task InternalNotifyAsync<T>(IClientProxy proxy, string methodName, T serverEvent) where T : ServerEvent
    {
        return proxy
            .SendEventAsync(methodName, serverEvent)
            .ContinueWith(prevTask =>
            {
                if (prevTask.Exception == null)
                {
                    _logger.LogInformation("{handler} successfully notified client", GetType().Name);
                }
                else
                {
                    _logger.LogError(prevTask.Exception, "{handler} failed to notify client", GetType().Name);
                }

                return Task.CompletedTask;
            });
    }
}