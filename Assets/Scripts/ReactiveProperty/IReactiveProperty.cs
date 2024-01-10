using System;

namespace ReactiveProperty
{
    public interface IReactiveProperty<T>
    {
        //event Action<T> OnValueChanged;
        T Value { get; set; }
        void Subscribe(Action<T> subscriber, bool triggerUponSubscription = true);
        void Unsubscribe(Action<T> subscriber);
    }
}