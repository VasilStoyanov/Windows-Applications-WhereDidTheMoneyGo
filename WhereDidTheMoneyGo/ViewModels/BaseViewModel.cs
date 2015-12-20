namespace WhereDidTheMoneyGo.ViewModels
{
    using System;
    using System.ComponentModel;
    using System.Linq;

    public class BaseViewModel : INotifyPropertyChanged
    {
        protected void NotifyOnPropertyChange(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}