using System;

namespace DrawerContainerViewControllerTest
{
	public partial class LeftTestTableViewController : TestTableViewController
	{
		public LeftTestTableViewController (IntPtr handle) : base (handle) { }

		public override string ReuseId => "LeftTestTableViewCell";
	}
}