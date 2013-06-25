using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Inputs.RawDevices;
using Daramkun.Liqueur.Inputs.States;
using Daramkun.Liqueur.Platforms;
using Windows.Devices.Sensors;

namespace Daramkun.Liqueur.Inputs.Devices
{
	public class Accelerometer : RawAccelerometer
	{
		Windows.Devices.Sensors.Accelerometer accelerometer;

		public Accelerometer ( IWindow window )
			: base ( window )
		{
			accelerometer = Windows.Devices.Sensors.Accelerometer.GetDefault ();
		}

		public override bool IsConnected
		{
			get { return true; }
		}

		private void ReadingChange ( Windows.Devices.Sensors.Accelerometer sender, AccelerometerReadingChangedEventArgs args )
		{
			AccelerometerValue.X = ( float ) args.Reading.AccelerationX;
			AccelerometerValue.Y = ( float ) args.Reading.AccelerationY;
			AccelerometerValue.Z = ( float ) args.Reading.AccelerationZ;
		}

		public override void Start ()
		{
			accelerometer.ReadingChanged += ReadingChange;
		}

		public override void Stop ()
		{
			accelerometer.ReadingChanged -= ReadingChange;
		}
	}
}
