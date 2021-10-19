using System;
using System.ComponentModel;
using Prism.Mvvm;
using Prism.Navigation;

namespace ArStart.ViewModels
{
    public class BaseViewModel : BindableBase
    {

        public BaseViewModel(INavigationService navigationService)
        {
            NavigationService = navigationService;
        }

        public INavigationService NavigationService;

    }
}