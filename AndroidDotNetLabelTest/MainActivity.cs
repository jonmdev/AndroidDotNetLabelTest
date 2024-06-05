using Android.Graphics;
using Android.Text;
using Android.Util;
using System.Diagnostics;
using System.Numerics;
using static Android.Views.ViewGroup;

namespace AndroidDotNetLabelTest {

    public static class  GraphicsExtensions {
        //====================================================================================
        //FUNCTION COPIED FROM MAUI'S GRAPHICSEXTENSIONS FOR CHECKING LINES FOR MAX WIDTH
        //====================================================================================
        public static Vector2 GetTextSizeAsSizeF(this Android.Text.StaticLayout target, bool hasBoundedWidth) {
            // We need to know if the static layout was created with a bounded width, as this is what
            // StaticLayout.Width returns.
            if (hasBoundedWidth)
                return new Vector2(target.Width, target.Height);

            float maxWidth = 0;
            int lineCount = target.LineCount;

            for (int i = 0; i < lineCount; i++) {
                float lineWidth = target.GetLineWidth(i);
                if (lineWidth > maxWidth) {
                    maxWidth = lineWidth;
                }
            }
            return new Vector2(maxWidth, target.Height);
        }

    }

    [Activity(Label = "@string/app_name", MainLauncher = true)]
    public class MainActivity : Android.App.Activity {
        protected override void OnCreate(Bundle? savedInstanceState) {
            base.OnCreate(savedInstanceState);


            //=====================================
            //BACKGROUND SETUP
            //=====================================
            AbsoluteLayout absRoot = new(this);
            absRoot.SetBackgroundColor(Android.Graphics.Color.AliceBlue);
            Android.Views.ViewGroup.LayoutParams linLayoutParam = new(LayoutParams.MatchParent, LayoutParams.MatchParent);
            SetContentView(absRoot, linLayoutParam);

            //=====================================
            //CREATE LABEL ON SCREEN
            //=====================================
            Android.Util.DisplayMetrics metrics = new DisplayMetrics();
            this.Display.GetMetrics(metrics);
            var scaledDensity = metrics.ScaledDensity;
            Debug.WriteLine("SCALED DENSITY " + scaledDensity); // = 2.75 on Pixel 5 emulator

            string testString = "HELLO HELLO HELLO HELLO HELLO HELLO HELLO HELLO HELLO";
            float fontSize = 20;
            int maxWidth = 1000;
            
            Typeface montserrateTypeFace = Typeface.CreateFromAsset(Resources.Assets, "montserratextrabold.otf");
            TextView text = new(this);
            text.SetText(testString, TextView.BufferType.Normal); //https://stackoverflow.com/questions/29861302/different-of-settextcharsequence-textview-buffertype-and-settextcharsequence
            text.TextSize = fontSize / scaledDensity; //interpreted in "scaled pixel units"
            text.SetTypeface(montserrateTypeFace, TypefaceStyle.Normal);
            text.SetMaxWidth(maxWidth);
            text.SetPadding(0, 0, 0, 0);
            text.SetBackgroundColor(Android.Graphics.Color.Green);
            absRoot.AddView(text);

            Android.Text.TextPaint textPaint = new();
            textPaint.TextSize = fontSize;

            textPaint.SetTypeface(montserrateTypeFace);  //https://stackoverflow.com/questions/39082855/how-do-i-use-a-custom-font-in-xamarin-android

            //===================
            //TIMER FUNCTION
            //===================
            double appTime = 0;
            double deltaTime = 0;
            DateTime dateTime = DateTime.Now;

            System.Threading.Timer timer = new(delegate {
                RunOnUiThread(delegate { //note timer is otherwise not main thread:
            
                    Debug.WriteLine("TIMER UPDATE FPS: " + (1 / (Math.Max(1E-22, deltaTime))));
                    deltaTime = (DateTime.Now - dateTime).TotalSeconds;
                    dateTime = DateTime.Now;
                    appTime += deltaTime;

                    //======================
                    //DEBUG THE SIZES
                    //======================
                    if (absRoot.MeasuredWidth > 0 && absRoot.MeasuredHeight > 0) {

                        //=================================
                        //DEBUG TEXT SIZE ON SCREEN
                        //=================================
                        Debug.WriteLine("TEXT SIZE  W" + text.Width + " H " + text.Height);

                        //=================================
                        //DEBUG STATIC LAYOUT 
                        //=================================
                        StaticLayout staticLayout = new(testString, textPaint, maxWidth, Layout.Alignment.AlignNormal, 1, 0, true); //should be "true" here to get same height as textview
                        var staticLayoutSize = staticLayout.GetTextSizeAsSizeF(false); //IF THIS IS NOT FALSE RETURNS FULL WIDTH OF "MAX WIDTH = 5000"
                        Debug.WriteLine("STATIC LAYOUT W " + staticLayoutSize.X + " H " + staticLayoutSize.Y + " top pad " + staticLayout.TopPadding + " bot pad " + staticLayout.BottomPadding);

                    }
                });
            });
            double timerFPS = .1;
            double timerIntervalS = 1.0 / timerFPS;
            double timerIntervalMS = timerIntervalS * 1000;
            timer.Change(0, (int)timerIntervalMS);


        }


        //notes:
        //absBox.LayoutParameters = new(absRoot.MeasuredWidth, (int)heightVal);
        //https://stackoverflow.com/questions/21045198/android-view-viewgrouplayoutparams-cannot-be-cast-to-android-widget-abslistview
        //absBox.LayoutParameters = new(500, 500);
        //https://stackoverflow.com/questions/41779934/how-is-staticlayout-used-in-android
        //https://stackoverflow.com/questions/10852431/how-to-reliably-determine-the-width-of-a-multi-line-string
        //var assetList = Assets.List("");
        //for (int i=0; i < assetList.Count(); i++) {
        //Debug.WriteLine("ASSET " + i + " " + assetList[i]); ;
        //}
    }
}