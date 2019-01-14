using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PhotoTaker
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();

            photoTakerView.FilesSaved += PhotoTakerView_FilesSaved;
            photoTakerView.Closed += PhotoTakerView_Closed;
            // saveButton.Clicked += SaveButton_Clicked;
        }

        void PhotoTakerView_Closed(object sender, EventArgs e)
        {
            this.Navigation.PopModalAsync();
        }

        void SaveButton_Clicked(object sender, EventArgs e)
        {
            photoTakerView.SaveFilesCommand?.Execute(null);
        }

        void Handle_Clicked(object sender, System.EventArgs e)
        {
            photoTakerView.CameraSwitchVisible = !photoTakerView.CameraSwitchVisible;
        }

        void PhotoTakerView_FilesSaved(object sender, EventArgs e)
        {
            var files = photoTakerView.FileNames;

            foreach (var file in files)
            {
                if (File.Exists(file)) 
                {
                    var fileInfo = new FileInfo(file);
                    var x = fileInfo.Length;
                }
            }
        }
    }
}
