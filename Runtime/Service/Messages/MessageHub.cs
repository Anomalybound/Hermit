using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Hermit.Service.Messages
{
    /// <summary>
    /// An implementation of the <c>Event Aggregator</c> pattern.
    /// </summary>
    public sealed class MessageHub : IMessageHub
    {
        private readonly Subscriptions _subscriptions;
        private Dictionary<Guid, Action<Type, object>> _globalHandlers = new Dictionary<Guid,  Action<Type, object>>();
        private Dictionary<Guid, Action<Guid, Exception>> _globalErrorHandlers = new Dictionary<Guid, Action<Guid, Exception>>();

        /// <summary>
        /// Creates an instance of the <see cref="MessageHub"/>.
        /// </summary>
        public MessageHub() => _subscriptions = new Subscriptions();

        /// <summary>
        /// Registers a callback which is invoked on every message published by the <see cref="MessageHub"/>.
        /// <remarks>Invoking this method with a new <paramref name="onMessage"/>overwrites the previous one.</remarks>
        /// </summary>
        /// <param name="onMessage">
        /// The callback to invoke on every message
        /// <remarks>The callback receives the type of the message and the message as arguments</remarks>
        /// </param>
        public Guid RegisterGlobalHandler(Action<Type, object> onMessage)
        {
            EnsureNotNull(onMessage);
            var key = Guid.NewGuid();
            _globalHandlers[key] = onMessage;
            return key;
        }

        /// <summary>
        /// Unsubscribes a global message handler from the <see cref="IMessageHub"/>.
        /// </summary>
        /// <param name="token">The token representing the handler</param>
        public void UnregisterGlobalHandler(Guid token)
        {
            _globalHandlers.Remove(token);
        }

        /// <summary>
        /// Invoked if an error occurs when publishing a message to a subscriber.
        /// <remarks>Invoking this method with a new <paramref name="onError"/>overwrites the previous one.</remarks>
        /// </summary>
        public Guid RegisterGlobalErrorHandler(Action<Guid, Exception> onError)
        {
            EnsureNotNull(onError);
            var key = Guid.NewGuid();
            _globalErrorHandlers[key] = onError;
            return key;
        }

        /// <summary>
        /// Unsubscribes a global message handler from the <see cref="IMessageHub"/>.
        /// </summary>
        /// <param name="token">The token representing the handler</param>
        public void UnregisterGlobalErrorHandler(Guid token)
        {
            _globalErrorHandlers.Remove(token);
        }

        /// <summary>
        /// Publishes the <paramref name="message"/> on the <see cref="MessageHub"/>.
        /// </summary>
        /// <param name="message">The message to published</param>
        public void Publish<T>(T message)
        {
            var localSubscriptions = _subscriptions.GetTheLatestSubscriptions();

            var msgType = typeof(T);

            foreach (var globalHandler in _globalHandlers.Values)
            {
                globalHandler?.Invoke(msgType, message);
            }

            // ReSharper disable once ForCanBeConvertedToForeach | Performance Critical
            for (var idx = 0; idx < localSubscriptions.Count; idx++)
            {
                var subscription = localSubscriptions[idx];

                if (!subscription.Type.IsAssignableFrom(msgType)) { continue; }

                try { subscription.Handle(message); }
                catch (Exception e)
                {
                    foreach (var globalErrorHandler in _globalErrorHandlers.Values)
                    {
                        globalErrorHandler?.Invoke(subscription.Token, e);
                    }
                }
            }
        }

        /// <summary>
        /// Subscribes a callback against the <see cref="MessageHub"/> for a specific type of message.
        /// </summary>
        /// <typeparam name="T">The type of message to subscribe to</typeparam>
        /// <param name="action">The callback to be invoked once the message is published on the <see cref="MessageHub"/></param>
        /// <returns>The token representing the subscription</returns>
        public Guid Subscribe<T>(Action<T> action) => Subscribe(action, TimeSpan.Zero);

        /// <summary>
        /// Subscribes a callback against the <see cref="MessageHub"/> for a specific type of message.
        /// </summary>
        /// <typeparam name="T">The type of message to subscribe to</typeparam>
        /// <param name="action">The callback to be invoked once the message is published on the <see cref="MessageHub"/></param>
        /// <param name="throttleBy">The <see cref="TimeSpan"/> specifying the rate at which subscription is throttled</param>
        /// <returns>The token representing the subscription</returns>
        public Guid Subscribe<T>(Action<T> action, TimeSpan throttleBy)
        {
            EnsureNotNull(action);
            return _subscriptions.Register(throttleBy, action);
        }

        /// <summary>
        /// Unsubscribes a subscription from the <see cref="MessageHub"/>.
        /// </summary>
        /// <param name="token">The token representing the subscription</param>
        public void Unsubscribe(Guid token) => _subscriptions.UnRegister(token);

        /// <summary>
        /// Checks if a specific subscription is active on the <see cref="MessageHub"/>.
        /// </summary>
        /// <param name="token">The token representing the subscription</param>
        /// <returns><c>True</c> if the subscription is active otherwise <c>False</c></returns>
        public bool IsSubscribed(Guid token) => _subscriptions.IsRegistered(token);

        /// <summary>
        /// Clears all the subscriptions from the <see cref="MessageHub"/>.
        /// <remarks>The global handler and the global error handler are not affected</remarks>
        /// </summary>
        public void ClearSubscriptions() => _subscriptions.Clear(false);

        /// <summary>
        /// Disposes the <see cref="MessageHub"/>.
        /// </summary>
        public void Dispose()
        {
            _globalHandlers.Clear();
            _globalHandlers = null;
            
            _globalErrorHandlers.Clear();
            _globalErrorHandlers = null;
            
            _subscriptions.Clear(true);
        }

        [DebuggerStepThrough]
        private static void EnsureNotNull(object obj)
        {
            if (obj is null) { throw new NullReferenceException(nameof(obj)); }
        }
    }
}
