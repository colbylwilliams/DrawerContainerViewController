namespace DrawerContainerViewController
{
	public class DrawerTransitioningEventArgs
	{
		public DrawerTransitioningState State { get; private set; }

		public DrawerTransitioningEventArgs (DrawerTransitioningState state)
		{
			State = state;
		}
	}
}