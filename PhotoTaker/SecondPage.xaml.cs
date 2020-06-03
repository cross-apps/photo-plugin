using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace PhotoTaker
{
    public partial class SecondPage : ContentPage
    {
        public SecondPage()
        {
            InitializeComponent();
        }

        void Handle_Clicked(object sender, System.EventArgs e)
        {
            Navigation.PushModalAsync(new MainPage());
        }
    }
}
