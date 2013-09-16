using System;
using Daramkun.Liqueur.Inputs.RawDevice;
using Daramkun.Liqueur.Platforms;
using Daramkun.Liqueur.Inputs.State;
using Daramkun.Liqueur.Mathematics;
using MonoTouch.CoreMotion;
using MonoTouch.Foundation;

namespace Daramkun.Liqueur.Inputs
{
	public class Accelerometer : AccelerometerDevice
	{
		MonoTouch.CoreMotion.CMMotionManager motionManager;
		float x, y, z;

		public Accelerometer ( IWindow window )
		{
			motionManager = new MonoTouch.CoreMotion.CMMotionManager ();
			motionManager.AccelerometerUpdateInterval = 1;
		}

		public override void Start ()
		{
			motionManager.StartAccelerometerUpdates ( new MonoTouch.Foundation.NSOperationQueue (), ( CMAccelerometerData accelerometerData, NSError error ) =>
			{
				x = accelerometerData.Acceleration.X; y = accelerometerData.Acceleration.Y; z = accelerometerData.Acceleration.Z;
			} );
		}

		public override void Stop ()
		{
			motionManager.StopAccelerometerUpdates ();
		}

		protected override AccelerometerState GenerateState ()
		{
			return new AccelerometerState ( new Vector3 ( x, y, z ) );
		}

		public override bool IsSupport { get { return motionManager.AccelerometerAvailable; } }
		public override bool IsConnected { get { return IsSupport; } }
	}
}

