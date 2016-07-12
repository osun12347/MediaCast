using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Casting;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace MediaCast
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        CastingDevicePicker castingPicker;
        public MainPage()
        {
            this.InitializeComponent();
            //Initialize our picker object
            castingPicker = new CastingDevicePicker();

            //Set the picker to filter to video capable casting devices
            castingPicker.Filter.SupportsVideo = true;

            //Hook up device selected event
            castingPicker.CastingDeviceSelected += CastingPicker_CastingDeviceSelected;

        }

        private async void CastingPicker_CastingDeviceSelected(CastingDevicePicker sender, CastingDeviceSelectedEventArgs args)
        {
            //Casting must occur from the UI thread.  This dispatches the casting calls to the UI thread.
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
            {
                //Create a casting conneciton from our selected casting device
                CastingConnection connection = args.SelectedCastingDevice.CreateCastingConnection();

                //Hook up the casting events
                connection.ErrorOccurred += Connection_ErrorOccurred;
                connection.StateChanged += Connection_StateChanged;

                //Cast the content loaded in the media element to the selected casting device
                await connection.RequestStartCastingAsync(mediaElement.GetAsCastingSource());
            });
        }

        private async void Connection_StateChanged(CastingConnection sender, object args)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                Debug("Casting Connection State Changed: " + sender.State);
            });
        }

        private void Debug(string v)
        {
            throw new NotImplementedException();
        }

        private async void Connection_ErrorOccurred(CastingConnection sender, CastingConnectionErrorOccurredEventArgs args)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                Debug("Casting Connection State Changed: " + sender.State);
            });

        }

        private void castPickerButton_Click(object sender, RoutedEventArgs e)
        {
            //Retrieve the location of the casting button
            GeneralTransform transform = castPickerButton.TransformToVisual(Window.Current.Content as UIElement);
            Point pt = transform.TransformPoint(new Point(0, 0));

            //Show the picker above our casting button
            castingPicker.Show(new Rect(pt.X, pt.Y, castPickerButton.ActualWidth, castPickerButton.ActualHeight),
                Windows.UI.Popups.Placement.Above);
        }
    }
}
