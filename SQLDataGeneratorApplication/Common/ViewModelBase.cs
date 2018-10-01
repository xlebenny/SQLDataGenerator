using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace SQLDataGeneratorApplication
{
    //ref https://stackoverflow.com/questions/36149863/how-to-write-viewmodelbase-in-mvvm-wpf

    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected virtual bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = "")
        {
            if (EqualityComparer<T>.Default.Equals(storage, value))
                return false;
            storage = value;
            this.OnPropertyChanged(propertyName);
            this.BindPropertyChanged(value, propertyName);
            return true;
        }

        protected void BindPropertyChanged(object value, string propertyName)
        {
            EmitPropertyChangedHelper.EmitPropertyChanged(this, (dynamic)value, propertyName);
        }

        private class EmitPropertyChangedHelper
        {
            private static readonly Action<ViewModelBase, string> CallPropertyChange = (viewModelBase, propertyName) => { viewModelBase.OnPropertyChanged(propertyName); };

            private static readonly Func<ViewModelBase, PropertyChangedEventHandler, bool> EventAlreadyAdded =
                (viewModelBase, handler) =>
                    viewModelBase.PropertyChanged != null
                    && viewModelBase.PropertyChanged.GetInvocationList().Any(x => x.Target == handler.Target);

            private static PropertyChangedEventHandler GetHandler(ViewModelBase caller, string propertyName)
            {
                return (obj, eventArgs) => { CallPropertyChange(caller, propertyName); };
            }

            public static void EmitPropertyChanged(ViewModelBase caller, object obj, string propertyName)
            {
                //do nothing
            }

            public static void EmitPropertyChanged(ViewModelBase caller, ViewModelBase anotherViewModelBase, string propertyName)
            {
                var handler = GetHandler(caller, propertyName);
                if (!EventAlreadyAdded(anotherViewModelBase, GetHandler(caller, propertyName)))
                    anotherViewModelBase.PropertyChanged += handler;
            }

            public static void EmitPropertyChanged<T>(ViewModelBase caller, ObservableCollection<T> observableCollection, string propertyName)
                where T : class
            {
                var isViewModelBase = typeof(ViewModelBase).IsAssignableFrom(typeof(T));
                if (!isViewModelBase) return; //C# no generic type overload

                //TODO maybe added once more
                observableCollection.CollectionChanged += (sender, args) =>
                {
                    if (args.NewItems == null) return;
                    foreach (ViewModelBase item in args.NewItems)
                        EmitPropertyChanged(caller, item, propertyName);
                };

                //benny --- for exist items, also need to PropertyChanged
                foreach (var item in observableCollection.OfType<ViewModelBase>())
                {
                    if (!EventAlreadyAdded(item, GetHandler(caller, propertyName)))
                    {
                        EmitPropertyChanged(caller, item, propertyName);

                        //and because haven't run before
                        CallPropertyChange(item, propertyName);
                    }
                }
            }
        }
    }
}