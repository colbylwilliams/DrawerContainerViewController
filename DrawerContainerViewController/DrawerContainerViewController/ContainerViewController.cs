using System;
using System.Threading.Tasks;

using CoreGraphics;
using UIKit;

namespace DrawerContainerViewController
{
	public class ContainerViewController : UIViewController
	{
		UIViewController LeftViewController;
		UIViewController MainViewController;

		UIPanGestureRecognizer LtrPanGestureRecognizer;
		UITapGestureRecognizer TapGestureRecognizer;

		bool pendingStatusBarAppearanceUpdate;

		static CGRect LeftViewControllerFrame => UIScreen.MainScreen.Bounds;
		static CGRect MainViewControllerFrame => UIScreen.MainScreen.Bounds;

		static CGRect MainViewControllerFrameOpen => new CGRect (DrawerWidth, MainViewControllerFrame.Y, MainViewControllerFrame.Width, MainViewControllerFrame.Height);


		public static nfloat DrawerWidth { get; set; } = 300;

		public bool HideStatusBarOnDrawerOpen { get; set; } = true;

		public bool DrawerOpen => MainViewController?.View.Frame == MainViewControllerFrameOpen;


		#region Constuctors

		public ContainerViewController ()
		{
			commonInit ();
		}


		void commonInit ()
		{
			View.Frame = MainViewControllerFrame;
		}

		#endregion


		#region LifeCycle

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			TapGestureRecognizer = new UITapGestureRecognizer (handleTapGesture) { Enabled = false };
			LtrPanGestureRecognizer = new UIPanGestureRecognizer (handleLtrPanGesture) { Delegate = new PanGestureRecognizerDelegate () };
		}


		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			MainViewController?.View.AddGestureRecognizer (TapGestureRecognizer);
			MainViewController?.View.AddGestureRecognizer (LtrPanGestureRecognizer);
		}


		public override void ViewWillDisappear (bool animated)
		{
			MainViewController?.View.RemoveGestureRecognizer (TapGestureRecognizer);
			MainViewController?.View.RemoveGestureRecognizer (LtrPanGestureRecognizer);

			base.ViewWillDisappear (animated);
		}

		#endregion


		#region StatusBar

		public override bool PrefersStatusBarHidden () => HideStatusBarOnDrawerOpen && MainViewController?.View.Frame.X > 0;

		public override UIStatusBarAnimation PreferredStatusBarUpdateAnimation => UIStatusBarAnimation.Slide;

		void updateStatusBarAppearance () => UIView.Animate (0.33, () => SetNeedsStatusBarAppearanceUpdate ());

		#endregion


		#region Open/Close Drawer

		public void CloseDrawer ()
		{
			TapGestureRecognizer.Enabled = false;

			UIView.AnimateNotify (0.2, () => {

				MainViewController.View.Frame = MainViewControllerFrame;

			}, (finished) => {

				updateStatusBarAppearance ();
			});
		}


		public async Task<bool> CloseDrawerAsync ()
		{
			TapGestureRecognizer.Enabled = false;

			var finished = await UIView.AnimateNotifyAsync (0.2, () => {
				MainViewController.View.Frame = MainViewControllerFrame;
			});

			updateStatusBarAppearance ();

			return finished;
		}

		#endregion


		#region GestureRecognizers

		void handleTapGesture (UITapGestureRecognizer tap) => CloseDrawer ();


		void handleLtrPanGesture (UIPanGestureRecognizer pan)
		{
			switch (pan.State) {
				case UIGestureRecognizerState.Began:

					pendingStatusBarAppearanceUpdate = true;

					break;

				case UIGestureRecognizerState.Changed:

					var ltrCurrentTranslation = pan.TranslationInView (pan.View);

					// if panning right-to-left ltrCurrentTranslation.X will be negative
					// setting pan.Enabled to false causes this method to be called again with 
					// pan.State == Cancelled, so we'll reenable the gesture in Cancelled: case
					if (ltrCurrentTranslation.X < 0) {
						pan.Enabled = false;
						return;
					}

					// if panning up or down more than left-to-right, kill this gesture recognizer
					// to allow only the MainViewControllers scrollview, tableview, etc. to scroll 
					if (NMath.Abs (ltrCurrentTranslation.Y) > ltrCurrentTranslation.X) {
						pan.Enabled = false;
						return;
					}

					var frame = MainViewControllerFrame;

					frame.X += ltrCurrentTranslation.X;

					MainViewController.View.Frame = frame;

					if (pendingStatusBarAppearanceUpdate) {

						pendingStatusBarAppearanceUpdate = false;

						updateStatusBarAppearance ();
					}

					break;

				case UIGestureRecognizerState.Ended:
				case UIGestureRecognizerState.Cancelled:

					pan.Enabled = true;

					// if the drawer has been opened at least 50%, finish opening, otherwise close
					var finalFrame = MainViewController.View.Frame.X > (DrawerWidth / 2) ? MainViewControllerFrameOpen : MainViewControllerFrame;

					UIView.AnimateNotify (0.1, () => {

						MainViewController.View.Frame = finalFrame;

					}, (finished) => {

						TapGestureRecognizer.Enabled = DrawerOpen;

						updateStatusBarAppearance ();
					});

					break;
			}
		}

		#endregion


		#region ChildViewControllers

		public void SetMainViewController (UIViewController mainViewController)
		{
			MainViewController = mainViewController;

			AddChildViewController (MainViewController);

			MainViewController.View.Frame = MainViewControllerFrame;

			View.AddSubview (MainViewController.View);

			MainViewController.DidMoveToParentViewController (this);
		}


		public void SetLeftViewController (UIViewController leftViewController)
		{
			LeftViewController = leftViewController;

			AddChildViewController (LeftViewController);

			LeftViewController.View.Frame = LeftViewControllerFrame;

			if (MainViewController?.View.IsDescendantOfView (View) ?? false) {
				View.InsertSubviewBelow (LeftViewController.View, MainViewController.View);
			} else {
				View.AddSubview (LeftViewController.View);
			}

			LeftViewController.DidMoveToParentViewController (this);
		}

		#endregion
	}


	public class PanGestureRecognizerDelegate : UIGestureRecognizerDelegate
	{
		public override bool ShouldRecognizeSimultaneously (UIGestureRecognizer gestureRecognizer, UIGestureRecognizer otherGestureRecognizer) => true;
	}
}