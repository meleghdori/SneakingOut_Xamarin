using SneakingOut_Xamarin.Services;
using SneakingOut_Xamarin.View;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SneakingOut_Xamarin
{
    public partial class App : Application
    {

        public App()
        {
            InitializeComponent();

            DependencyService.Register<MockDataStore>();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
