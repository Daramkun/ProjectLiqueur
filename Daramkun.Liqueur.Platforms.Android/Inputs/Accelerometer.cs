using System;
using Daramkun.Liqueur.Inputs.RawDevice;
using Daramkun.Liqueur.Platforms;
using Daramkun.Liqueur.Inputs.State;
using Daramkun.Liqueur.Mathematics;

namespace Daramkun.Liqueur.Inputs
{
	public class Accelerometer : AccelerometerDevice
	{
		private class InternalAccelerometerListener : Android.Hardware.ISensorEventListener
		{
			public float x, y, z;
			public void OnAccuracyChanged ( Android.Hardware.Sensor sensor, Android.Hardware.SensorStatus accuracy ) { }
			public void OnSensorChanged ( Android.Hardware.SensorEvent e )
			{ x = e.Values [ 0 ]; y = e.Values [ 1 ]; z = e.Values [ 2 ]; }
			public void Dispose () { }
			public IntPtr Handle { get { return new IntPtr ( 0 ); } }
		}

		Android.Hardware.SensorManager sensorManager;
		Android.Hardware.Sensor sensor;

		InternalAccelerometerListener listener;

		public Accelerometer ( IWindow window )
		{
			OpenTK.Platform.Android.AndroidGameView gameView = ( window.Handle as OpenTK.Platform.Android.AndroidGameView );
			sensorManager = gameView.Context.GetSystemService ( Android.Content.Context.SensorService ) as Android.Hardware.SensorManager;
			sensor = sensorManager.GetDefaultSensor ( Android.Hardware.SensorType.Accelerometer );
		}

		public override void Start ()
		{
			sensorManager.RegisterListener ( listener = new InternalAccelerometerListener (), sensor, Android.Hardware.SensorDelay.Game );
		}

		public override void Stop ()
		{
			sensorManager.UnregisterListener ( listener, sensor );
		}

		protected override AccelerometerState GenerateState ()
		{
			return new AccelerometerState ( new Vector3 ( listener.x, listener.y, listener.z ) );
		}

		public override bool IsSupport { get { return sensor != null; } }
		public override bool IsConnected { get { return IsSupport; } }
	}
}

