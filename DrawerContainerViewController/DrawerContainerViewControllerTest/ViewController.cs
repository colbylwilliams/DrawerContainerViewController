using System;

using UIKit;

using DrawerContainerViewController;

namespace DrawerContainerViewControllerTest
{
	public partial class ViewController : UIViewController
	{
		protected ViewController (IntPtr handle) : base (handle)
		{
			// Note: this .ctor should not contain any initialization logic.
		}

		partial void viewControllerExample (Foundation.NSObject sender)
		{
			var mainViewController = Storyboard.InstantiateViewController ("MainTestViewController") as MainTestViewController;
			var leftViewController = Storyboard.InstantiateViewController ("LeftTestViewController") as LeftTestViewController;

			var containerViewController = new ContainerViewController ();

			//containerViewController.HideStatusBarOnDrawerOpen = false;

			containerViewController.SetMainViewController (mainViewController);
			containerViewController.SetLeftViewController (leftViewController);

			PresentViewController (containerViewController, true, null);
		}

		partial void navigationControllerExample (Foundation.NSObject sender)
		{
			var mainViewController = new UINavigationController (Storyboard.InstantiateViewController ("MainTestTableViewController") as MainTestTableViewController);
			var leftViewController = Storyboard.InstantiateViewController ("LeftTestTableViewController") as LeftTestTableViewController;

			var containerViewController = new ContainerViewController ();

			//containerViewController.HideStatusBarOnDrawerOpen = false;

			containerViewController.SetMainViewController (mainViewController);
			containerViewController.SetLeftViewController (leftViewController);

			PresentViewController (containerViewController, true, null);
		}

		partial void tableViewControllerExample (Foundation.NSObject sender)
		{
			var mainViewController = Storyboard.InstantiateViewController ("MainTestTableViewController") as MainTestTableViewController;
			var leftViewController = Storyboard.InstantiateViewController ("LeftTestTableViewController") as LeftTestTableViewController;

			var containerViewController = new ContainerViewController ();

			//containerViewController.HideStatusBarOnDrawerOpen = false;

			containerViewController.SetMainViewController (mainViewController);
			containerViewController.SetLeftViewController (leftViewController);

			PresentViewController (containerViewController, true, null);
		}
	}
}