using System;

using UIKit;

using DrawerContainerViewController;

namespace DrawerContainerViewControllerTest
{
	public partial class ViewController : UIViewController
	{
		protected ViewController (IntPtr handle) : base (handle) { }

		partial void viewControllerExample (Foundation.NSObject sender)
		{
			var topViewController = Storyboard.InstantiateViewController ("MainTestViewController") as MainTestViewController;
			var leftViewController = Storyboard.InstantiateViewController ("LeftTestViewController") as LeftTestViewController;

			var containerViewController = new ContainerViewController (topViewController, leftViewController);

			containerViewController.DrawerTransitioning += (s, e) => Console.WriteLine ($"DrawerTransitioning :: {e.State}");

			PresentViewController (containerViewController, true, null);
		}

		partial void navigationControllerExample (Foundation.NSObject sender)
		{
			var topViewController = new UINavigationController (Storyboard.InstantiateViewController ("MainTestTableViewController") as MainTestTableViewController);
			var leftViewController = Storyboard.InstantiateViewController ("LeftTestTableViewController") as LeftTestTableViewController;

			var containerViewController = new ContainerViewController (topViewController, leftViewController);

			containerViewController.DrawerTransitioning += (s, e) => Console.WriteLine ($"DrawerTransitioning :: {e.State}");

			PresentViewController (containerViewController, true, null);
		}

		partial void tableViewControllerExample (Foundation.NSObject sender)
		{
			var topViewController = Storyboard.InstantiateViewController ("MainTestTableViewController") as MainTestTableViewController;
			var leftViewController = Storyboard.InstantiateViewController ("LeftTestTableViewController") as LeftTestTableViewController;

			var containerViewController = new ContainerViewController (topViewController, leftViewController);

			containerViewController.DrawerTransitioning += (s, e) => Console.WriteLine ($"DrawerTransitioning :: {e.State}");

			PresentViewController (containerViewController, true, null);
		}
	}
}