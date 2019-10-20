using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Prototype
{
    public class ObservableObject : INotifyPropertyChangedEx
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public event PropertyChangedExEventHandler PropertyChangedEx;

        protected void NotifyPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        protected void NotifyPropertyChangedEx(string propertyName, object newValue, object oldValue) =>
            PropertyChangedEx?.Invoke(this, propertyName, newValue, oldValue);

        protected bool SetPropertyField<T>(ref T propertyField, T value, [CallerMemberName] string propertyName = "unspecified_property_name")
        {
            if (EqualityComparer<T>.Default.Equals(propertyField, value))
                return false;

            object oldValue = propertyField;
            object newValue = value;
            propertyField = value;
            OnPropertyChanged(propertyName, newValue, oldValue);
            NotifyPropertyChangedEx(propertyName, newValue, oldValue);
            NotifyPropertyChanged(propertyName);
            return true;
        }

        protected bool HasPropertyChangedEventHandlers => PropertyChanged != null || PropertyChangedEx != null;

        protected virtual void OnPropertyChanged(string propertyName, object newValue, object oldValue)
        {
        }

        public void ForcePropertyValueUpdate<T>(string propertyName, T value)
        {
            var propertyInfo = GetType().GetProperty(propertyName);
            var oldValue = propertyInfo.GetValue(this);

            if (oldValue == null && value != null ||
                oldValue != null && (value == null || !EqualityComparer<T>.Default.Equals((T)oldValue, value)))
            {
                propertyInfo.SetValue(this, value);
            }
            else
            {
                OnPropertyChanged(propertyName, value, value);
                NotifyPropertyChangedEx(propertyName, value, value);
                NotifyPropertyChanged(propertyName);
            }
        }
    }
}
