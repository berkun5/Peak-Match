using System;
using System.Collections.Generic;
using System.Linq;

namespace ReactiveProperty
{
    public class ReactiveProperty<T> : IReactiveProperty<T>
    {
        private T _value;
        public event Action<T> OnValueChanged;

        public ReactiveProperty()
        {
            Value = default;
        }
        
        public ReactiveProperty(T initialValue)
        {
            _value = initialValue;
            OnValueChanged?.Invoke(_value);
        }

        public T Value
        {
            get => _value;
            set
            {
                if (!EqualityComparer<T>.Default.Equals(_value, value))
                {
                    _value = value;
                    OnValueChanged?.Invoke(value);
                }
            }
        }

        public void Subscribe(Action<T> subscriber, bool triggerUponSubscription = true)
        {
            if (OnValueChanged != null && OnValueChanged.GetInvocationList().Contains(subscriber))
            {
                return;
            }
            
            OnValueChanged += subscriber;
            
            if (triggerUponSubscription)
            {
                subscriber?.Invoke(_value);
            }
        }

        public void Unsubscribe(Action<T> subscriber)
        {
            OnValueChanged -= subscriber;
        }
    }
}