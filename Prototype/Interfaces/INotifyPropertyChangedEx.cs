using System.ComponentModel;

namespace Prototype.Interfaces
{
    public delegate void PropertyChangedExEventHandler(object sender, string propertyName, object newValue, object oldValue);

    public interface INotifyPropertyChangedEx : INotifyPropertyChanged
    {
        event PropertyChangedExEventHandler PropertyChangedEx;
    }
}
