using System;
using System.Threading.Tasks;

using CoreGraphics;
using UIKit;

namespace DrawerContainerViewController
{
	public class ContainerViewController : UIViewController
	{
		public event EventHandler<DrawerTransitioningEventArgs> DrawerTransitioning;

		UIViewController LeftViewController;
		UIViewController TopViewController;

		UIView TopView => TopViewController?.View;
		UIView LeftView => LeftViewController?.View;

		UIPanGestureRecognizer LtrPanGestureRecognizer;
		UITapGestureRecognizer TapGestureRecognizer;

		bool pendingStatusBarAppearanceUpdate;

		static CGRect LeftViewFrame => UIScreen.MainScreen.Bounds;
		static CGRect TopViewFrame => UIScreen.MainScreen.Bounds;

		static CGRect TopViewFrameOpen => new CGRect (DrawerWidth, TopViewFrame.Y, TopViewFrame.Width, TopViewFrame.Height);


		public static nfloat DrawerWidth { get; set; } = 300;

		public bool HideStatusBarOnDrawerOpen { get; set; } = true;

		public bool DrawerOpen => TopView?.Frame == TopViewFrameOpen;



		#region Constuctors

		public ContainerViewController ()
		{
			commonInit ();
		}

		public ContainerViewController (UIViewController topViewController, UIViewController leftViewController)
		{
			commonInit ();

			SetTopViewController (topViewController);
			SetLeftViewController (leftViewController);
		}

		void commonInit () => View.Frame = TopViewFrame;

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

			TopView?.AddGestureRecognizer (TapGestureRecognizer);
			TopView?.AddGestureRecognizer (LtrPanGestureRecognizer);
		}


		public override void ViewWillDisappear (bool animated)
		{
			TopView?.RemoveGestureRecognizer (TapGestureRecognizer);
			TopView?.RemoveGestureRecognizer (LtrPanGestureRecognizer);

			base.ViewWillDisappear (animated);
		}

		#endregion


		#region StatusBar

		public override bool PrefersStatusBarHidden () => HideStatusBarOnDrawerOpen && TopView?.Frame.X > 0;

		public override UIStatusBarAnimation PreferredStatusBarUpdateAnimation => UIStatusBarAnimation.Slide;

		void updateStatusBarAppearance () => UIView.Animate (0.33, () => SetNeedsStatusBarAppearanceUpdate ());

		#endregion


		#region Open/Close Drawer

		public void CloseDrawer ()
		{
			TapGestureRecognizer.Enabled = false;

			UIView.AnimateNotify (0.2, () => {

				TopView.Frame = TopViewFrame;

			}, (finished) => {

				DrawerTransitioning?.Invoke (this, new DrawerTransitioningEventArgs (DrawerTransitioningState.Closed));

				updateStatusBarAppearance ();

			});
		}


		public async Task<bool> CloseDrawerAsync ()
		{
			TapGestureRecognizer.Enabled = false;

			var finished = await UIView.AnimateNotifyAsync (0.2, () => {
				TopView.Frame = TopViewFrame;
			});

			DrawerTransitioning?.Invoke (this, new DrawerTransitioningEventArgs (DrawerTransitioningState.Closed));

			updateStatusBarAppearance ();

			return finished;
		}

		#endregion


		#region GestureRecognizers

		void handleTapGesture (UITapGestureRecognizer tap)
		{
			DrawerTransitioning?.Invoke (this, new DrawerTransitioningEventArgs (DrawerTransitioningState.Transitioning));

			CloseDrawer ();
		}


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
					// to allow only the TopViewControllers scrollview, tableview, etc. to scroll 
					if (NMath.Abs (ltrCurrentTranslation.Y) > ltrCurrentTranslation.X) {
						pan.Enabled = false;
						return;
					}

					var frame = TopViewFrame;

					frame.X += ltrCurrentTranslation.X;

					TopView.Frame = frame;

					if (pendingStatusBarAppearanceUpdate) {

						pendingStatusBarAppearanceUpdate = false;

						DrawerTransitioning?.Invoke (this, new DrawerTransitioningEventArgs (DrawerTransitioningState.Transitioning));

						updateStatusBarAppearance ();
					}

					break;

				case UIGestureRecognizerState.Ended:
				case UIGestureRecognizerState.Cancelled:

					pan.Enabled = true;

					// the TopView's frame was never moved because the pan was the wrong direction
					// or the pan was a vertical scroll, so no reason to do anything here
					if (pendingStatusBarAppearanceUpdate) {
						pendingStatusBarAppearanceUpdate = false;
						return;
					}

					// if the drawer has been opened at least 50%, finish opening, otherwise close
					var finalFrame = TopView.Frame.X > (DrawerWidth / 2) ? TopViewFrameOpen : TopViewFrame;

					UIView.AnimateNotify (0.1, () => {

						TopView.Frame = finalFrame;

					}, (finished) => {

						TapGestureRecognizer.Enabled = DrawerOpen;

						DrawerTransitioning?.Invoke (this, new DrawerTransitioningEventArgs (DrawerOpen ? DrawerTransitioningState.Open : DrawerTransitioningState.Closed));

						updateStatusBarAppearance ();
					});

					break;
			}
		}

		#endregion


		#region ChildViewControllers

		public void SetTopViewController (UIViewController mainViewController)
		{
			TopViewController = mainViewController;

			AddChildViewController (TopViewController);

			TopView.Frame = TopViewFrame;

			var layer = TopView.Layer;

			layer.ShadowPath = UIBezierPath.FromRect (TopView.Bounds).CGPath;
			layer.ShadowColor = UIColor.Black.CGColor;
			layer.ShadowOpacity = 1;
			layer.ShadowOffset = new CGSize (0, 2.5);
			layer.ShadowRadius = 2.5f;

			View.AddSubview (TopView);

			TopViewController.DidMoveToParentViewController (this);
		}


		public void SetLeftViewController (UIViewController leftViewController)
		{
			LeftViewController = leftViewController;

			AddChildViewController (LeftViewController);

			LeftView.Frame = LeftViewFrame;

			if (TopView?.IsDescendantOfView (View) ?? false) {

				View.InsertSubviewBelow (LeftView, TopView);

			} else {

				View.AddSubview (LeftView);
			}

			LeftViewController.DidMoveToParentViewController (this);
		}

		#endregion
	}
}