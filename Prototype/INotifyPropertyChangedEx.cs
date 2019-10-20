using System.ComponentModel;

namespace Prototype
{
    public delegate void PropertyChangedExEventHandler(object sender, string propertyName, object newValue, object oldValue);

    public interface INotifyPropertyChangedEx : INotifyPropertyChanged
    {
        event PropertyChangedExEventHandler PropertyChangedEx;
    }
}
