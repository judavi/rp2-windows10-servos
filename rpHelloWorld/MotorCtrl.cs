using System;
using System.Diagnostics;
using System.Threading;
using Windows.Devices.Gpio;
using Windows.Foundation;
using Windows.System.Threading;

namespace rpHelloWorld
{
    class MotorCtrl
    {
        const int LEFT_PWM_PIN = 5;
        const int RIGHT_PWM_PIN = 6;
        private static GpioController gpioController = null;
        private static GpioPin leftPwmPin = null;
        private static GpioPin rightPwmPin = null;
        private static ulong ticksPerMs;
        private static IAsyncAction workItemThread;
        private static bool GpioInitialized = false;
        private enum MotorIds { Left, Right };
        public enum PulseMs { stop = -1, ms1 = 0, ms2 = 2 } // values selected for thread-safety
        public static PulseMs waitTimeLeft = PulseMs.stop;
        public static PulseMs waitTimeRight = PulseMs.stop;
        public static int speedValue = 10000;

        public static void MotorsInit()
        {
            GpioInit();
            ticksPerMs = (ulong)(Stopwatch.Frequency) / 1000;
        }

        private static void GpioInit()
        {
            try
            {
                gpioController = GpioController.GetDefault();
                if (null != gpioController)
                {
                    leftPwmPin = gpioController.OpenPin(LEFT_PWM_PIN);
                    leftPwmPin.SetDriveMode(GpioPinDriveMode.Output);

                    rightPwmPin = gpioController.OpenPin(RIGHT_PWM_PIN);
                    rightPwmPin.SetDriveMode(GpioPinDriveMode.Output);
                    GpioInitialized = true;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("ERROR: GpioInit failed - " + ex.Message);
            }
        }

        /// <summary>
        /// Generate a single motor pulse wave for given servo motor (High for 1 to 2ms, duration for 20ms).
        /// motor value denotes which moter to send pulse to.
        /// </summary>
        /// <param name="motor"></param>
        private static void PulseMotor(MotorIds motor)
        { 
            //Values! Here we specify if we will write a value in the GPIO
            ulong pulseTicks = ticksPerMs;
            if (motor == MotorIds.Left)
            {
                if (waitTimeLeft == PulseMs.stop) return;
                if (waitTimeLeft == PulseMs.ms2) pulseTicks = ticksPerMs * 2;
                leftPwmPin.Write(GpioPinValue.High);
            }
            else
            {
                if (waitTimeRight == PulseMs.stop) return;
                if (waitTimeRight == PulseMs.ms2) pulseTicks = ticksPerMs * 2;
                rightPwmPin.Write(GpioPinValue.High);
            }

            // Timing 
            // Helps to control the direction of the movement
            // The delay of this count change the pulse to rotate in another direction
            ulong delta;
            ulong starttick = (ulong)(App.stopwatch.ElapsedTicks);
            while (true)
            {
                delta = (ulong)(App.stopwatch.ElapsedTicks) - starttick;
                if (delta > (20 * ticksPerMs)) break;
                if (delta > pulseTicks) break;
            }

            // At the end of pulse we ship a low voltage in the GPIO
            if (motor == MotorIds.Left) leftPwmPin.Write(GpioPinValue.Low);
            else rightPwmPin.Write(GpioPinValue.Low);
        }

        /// <summary>
        /// Move the servo for X ms 
        /// The direction of the servo it's controlled by the PulseMs values
        /// </summary>
        /// <param name="ms"></param>
        public static void MoveMotorsForTime(uint ms)
        {
            if (!GpioInitialized) return;

            ManualResetEvent mre = new ManualResetEvent(false);
            ulong stick = (ulong)App.stopwatch.ElapsedTicks;
            while (true)
            {
                ulong delta = (ulong)(App.stopwatch.ElapsedTicks) - stick;
                if (delta > (ms * ticksPerMs)) break;  // stop motion after given time

                PulseMotor(MotorIds.Left);
                mre.WaitOne(2);
                PulseMotor(MotorIds.Right);
                mre.WaitOne(2);
            }
        }

        public static void test()
        {
            leftPwmPin.Write(GpioPinValue.High);
            leftPwmPin.Write(GpioPinValue.Low);
        }
    }
}