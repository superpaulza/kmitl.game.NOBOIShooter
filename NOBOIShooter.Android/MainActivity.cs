using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Microsoft.Xna.Framework;

namespace NOBOIShooter.Android
{
    [Activity(
        Label = "NOBOI Shooter",
        Icon = "@drawable/icon",
        Theme = "@style/Theme.Splash",
        MainLauncher = true,
        ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize | ConfigChanges.Keyboard | ConfigChanges.KeyboardHidden,
        ScreenOrientation = ScreenOrientation.Landscape)]
    public class MainActivity : AndroidGameActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Immersive fullscreen: hide status/nav bars so the 1280x720
            // virtual area maps cleanly to the whole display.
            try
            {
                Window?.AddFlags(WindowManagerFlags.Fullscreen | WindowManagerFlags.KeepScreenOn);
                if (Window != null)
                {
#pragma warning disable CS0618 // SystemUiVisibility is deprecated but still required for wide device support
                    var decor = Window.DecorView;
                    if (decor != null && Build.VERSION.SdkInt < BuildVersionCodes.R)
                    {
                        decor.SystemUiVisibility = (StatusBarVisibility)(
                            SystemUiFlags.Fullscreen | SystemUiFlags.HideNavigation | SystemUiFlags.ImmersiveSticky);
                    }
#pragma warning restore CS0618
                }
            }
            catch { /* non-fatal cosmetics */ }

            var game = new Main();
            SetContentView((View)game.Services.GetService(typeof(View)));
            game.Run();
        }
    }
}
