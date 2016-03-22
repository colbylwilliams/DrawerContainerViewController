using System;
using System.Collections.Generic;

using Foundation;
using UIKit;

namespace DrawerContainerViewControllerTest
{
	public partial class LeftTestTableViewController : UITableViewController
	{

		public LeftTestTableViewController (IntPtr handle) : base (handle)
		{
		}

		static string ReuseId = "LeftTestTableViewCell";

		public List<string> Items { get; set; }

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			Items = new List<string> ();

			for (int i = 0; i < 30; i++)
				Items.Add ($"{ReuseId} {i}");
		}

		public override nint NumberOfSections (UITableView tableView) => 1;

		public override nint RowsInSection (UITableView tableView, nint section) => Items?.Count ?? 0;

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			var cell = tableView.DequeueReusableCell (ReuseId, indexPath);

			cell.TextLabel.Text = Items [indexPath.Row];

			return cell;
		}
	}
}