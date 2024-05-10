using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Drawing;
using System.IO;

namespace Modelling___Computing_NEA
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    
    public partial class MainWindow : Window
    {
        Random rnd = new Random();
        Bitmap originalCircle = new Bitmap(@"..\..\..\Box, slanted.jpg");

        int settings_SampleCount = 20;    //  For testing
        double blurFactor = 2;
        bool circularRandom = true; //  For testing
        enum sampleType
        {
            circular, box, none 
        }
        sampleType sampleStatus = sampleType.circular;
        int pixelSideLength = 10; //  For testing, 40, 40 gives grey square
        public MainWindow()
        {
            InitializeComponent();
            updateLblSettings();
            imgOriginalImage.Source = BitmapToImageSource(originalCircle);
        }

        /*private long _contactNo;
        public long contactNo
        {
            get { return _contactNo; }
            set
            {
                if (value == _contactNo)
                    return;
                _contactNo = value;
                OnPropertyChanged();
            }
        }*/

        private void updateLblSettings()
        {
            
            lblSettings.Content = "Settings:\nPixel Res:" + pixelSideLength  + "     Samples:" + settings_SampleCount + "     Blur Factor: " + blurFactor;

        }

        private Bitmap drawSquare(Bitmap input, int x, int y, int width, int height, System.Drawing.Color colour)
        {
            for (int verticalDist = 0; verticalDist < width; verticalDist++)
            {
                for (int horizontalDist = 0; horizontalDist < height; horizontalDist++)
                {
                    input.SetPixel(x + horizontalDist, y + verticalDist, colour);
                }
            }
            return input;
        }

        private void btnConvert_Click(object sender, RoutedEventArgs e)
        {
            int transformedWidth = originalCircle.Width, transformedHeight = originalCircle.Height;
            Bitmap transformedImage = new Bitmap(transformedWidth, transformedHeight);
            for (int _x = 0; _x < transformedImage.Width; _x++)  //  INEFICIENT
            {
                for (int _y = 0; _y < transformedImage.Height; _y++)
                {

                    System.Drawing.Color newColor = System.Drawing.Color.FromArgb(255, 255, 255);
                    transformedImage.SetPixel(_x, _y, newColor);
                }
            }
            int samples = settings_SampleCount;
            if (sampleStatus == sampleType.none)
            {
                samples = 1;
            }



            double boxMag = 0;
            int boxSampleCount = 0;

            for (int x = 0; x < transformedWidth- pixelSideLength; x += pixelSideLength)
            {
                for (int y = 0; y < transformedHeight - pixelSideLength; y += pixelSideLength)
                {
                    int[] offset = new int[2] {0, 0};
                    
                    System.Drawing.Color[] locationColours;
                    int[] colours = new int[3] {0, 0, 0};
                    for (int i = 0; i < samples; i++)
                    {
                        if (sampleStatus == sampleType.circular)
                        {
                            double theta = rnd.Next(360);
                            double radius = rnd.Next(Convert.ToInt32(blurFactor * pixelSideLength));
                            offset[0] = Convert.ToInt32(radius * Math.Cos(theta * Math.PI / 180));
                            offset[1] = Convert.ToInt32(radius * Math.Sin(theta * Math.PI / 180));
                        }
                        else if (sampleStatus == sampleType.box)    //  0.66 is a constant offset that brings the bluring in line with circular bluring
                        {
                            int blurAsInt = Convert.ToInt32(blurFactor);
                            offset[0] = Convert.ToInt32(0.66D * blurAsInt * (rnd.Next(0, 2 * pixelSideLength) - pixelSideLength));
                            offset[1] = Convert.ToInt32(0.66D * blurAsInt * (rnd.Next(0, 2 * pixelSideLength) - pixelSideLength));
                            boxMag += Math.Sqrt(offset[0] * offset[0] + offset[1] * offset[1]);
                            boxSampleCount++;

                        }
                        else if(sampleStatus != sampleType.none)
                        {
                            MessageBox.Show("Sample type unrecognised");
                        }
                        int currX = x + pixelSideLength / 2 + offset[0];
                        int currY = y + pixelSideLength / 2 + offset[1];
                        currX = Math.Min(Math.Max(currX, 0), originalCircle.Width-1);
                        currY = Math.Min(Math.Max(currY, 0), originalCircle.Height-1);
                        System.Drawing.Color locationColour = originalCircle.GetPixel(currX, currY);
                        colours[0] += locationColour.R;
                        colours[1] += locationColour.G;
                        colours[2] += locationColour.B;
                    }
                    System.Drawing.Color locationColourFinal = System.Drawing.Color.FromArgb(colours[0] / samples, colours[1] / samples, colours[2] / samples);
                    transformedImage = drawSquare(transformedImage, x, y, pixelSideLength, pixelSideLength, locationColourFinal);

                }
            }
            imgTransformedImage.Source = BitmapToImageSource(transformedImage);
            //MessageBox.Show("Average box distance: " + boxMag / boxSampleCount);
        }

        private BitmapImage BitmapToImageSource(System.Drawing.Bitmap bitmap)   //  Copied from stack overflow
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();
                return bitmapimage;
            }
        }

        private void txtPixelRes_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!int.TryParse(e.Text, out _))
            {
                // If the input is not a valid integer, mark the event as handled
                e.Handled = true;
            }
        }

        private void txtPixelRes_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                pixelSideLength = Convert.ToInt32(txtPixelRes.Text);
                updateLblSettings();
                txtPixelRes.Text = "Pixel Res";
            }
        }

        private void txtSampleCount_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                settings_SampleCount = Convert.ToInt32(txtSampleCount.Text);
                updateLblSettings();
                txtSampleCount.Text = "Sample Count";
            }
        }
        private void txtSampleCount_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!int.TryParse(e.Text, out _))
            {
                // If the input is not a valid integer, mark the event as handled
                e.Handled = true;
            }
        }
        private void txtBlurFactor_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                blurFactor = Convert.ToInt32(txtBlurFactor.Text);
                updateLblSettings();
                txtBlurFactor.Text = "Blur Factor";
            }
        }

        private void txtBlurFactor_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!int.TryParse(e.Text, out _))
            {
                // If the input is not a valid integer, mark the event as handled
                e.Handled = true;
            }
        }

        private void setSampleMethodToCircular()
        {
            circularRandom = true;
        }

        private void cmbSampleMethod_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            sampleStatus = (sampleType)cmbSampleMethod.SelectedIndex;
        }
    }
}
