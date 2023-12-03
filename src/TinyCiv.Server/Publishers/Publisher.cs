using System.Collections.Concurrent;
using Microsoft.AspNetCore.SignalR;
using TinyCiv.Server.Core.Extensions;
using TinyCiv.Server.Core.Publishers;
using TinyCiv.Shared.Events.Server;

namespace TinyCiv.Server.Publishers;

public class Publisher : IPublisher
{
    private readonly ILogger<Publisher> _logger;
    private readonly IPublisherStorage _storage;
    
    public Publisher(IPublisherStorage storage, ILogger<Publisher> logger)
    {
        _logger = logger;
        _storage = storage;
    }
    
    public void Subscribe(Subscriber subscriber)
    {
        _storage.Add(subscriber);
    }

    public void Unsubscribe(Subscriber subscriber)
    {
        _storage.Remove(subscriber);
    }

    public Task NotifyAllAsync<T>(string methodName, T serverEvent) where T : ServerEvent
    {
        var notificationTasks = new List<Task>();

        var iterator = _storage.GetIterator();
        while (iterator.HasNext())
        {
            var subscriber = iterator.Next();
            
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