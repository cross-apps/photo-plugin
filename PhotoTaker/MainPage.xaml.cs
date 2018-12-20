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
            // saveButton.Clicked += SaveButton_Clicked;
        }

        void SaveButton_Clicked(object sender, EventArgs e)
        {
            photoTakerView.SaveFilesCommand?.Execute(null);
        }

        void PhotoTakerView_FilesSaved(object sender, EventArgs e)
        {
            var files = photoTakerView.FileNames;

            // var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            // var tmp = Path.Combine(documents, "..", "tmp");

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
