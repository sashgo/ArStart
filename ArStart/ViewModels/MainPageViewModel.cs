using System;
using System.Windows.Input;
using ArStart.Views;
using Prism.Commands;
using Prism.Navigation;

namespace ArStart.ViewModels
{
    public class StartPageViewModel : BaseViewModel
    {
        public StartPageViewModel(INavigationService navigationService) : base(navigationService)
        {
        }

        private ICommand _openArPageCommand;
        public ICommand OpenArPageCommand
        {
            get { return _openArPageCommand ?? (_openArPageCommand = new DelegateCommand(OpenArPage)); }
        }

        private async void OpenArPage()
        {
            await NavigationService.NavigateAsync(nameof(ArPage));
        }
    }
}
