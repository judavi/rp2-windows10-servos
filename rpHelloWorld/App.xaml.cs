using System.Diagnostics;
using Windows.UI.Xaml;

namespace rpHelloWorld
{
    /// <summary>
    /// This is a simplified version to learn how to controll servos on RP2
    /// and windows 10. It's based on https://www.hackster.io/windowsiot/robot-kit-6dd474
    /// but I remove all the innecesary code to help someone that's starting
    /// to understand how move the servos
    /// </summary>
    /// <remarks>Author: Judavi github.com/judavi</remarks>
    public sealed partial class App : Application
    {
        public static Stopwatch stopwatch;  

        /// <summary>
        /// This Windows 10 Universal app just run once and move the servo for 5 seconds
        /// and then change the direction of the movemente, run for another 5 seconds
        /// and finish
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            stopwatch = new Stopwatch();
            stopwatch.Start();
            MotorCtrl.MotorsInit();
            MotorCtrl.waitTimeLeft = MotorCtrl.PulseMs.ms2;
            MotorCtrl.waitTimeRight = MotorCtrl.PulseMs.ms1;
            MotorCtrl.speedValue = 10000;

            //Here we start the movement of the servos 
            MotorCtrl.MoveMotorsForTime(5000);

            //We change the directions changing the timing of the 
            //bit sendend
            MotorCtrl.waitTimeLeft = MotorCtrl.PulseMs.ms1;
            MotorCtrl.waitTimeRight = MotorCtrl.PulseMs.ms2;

            //Again we send movement to the servos
            MotorCtrl.MoveMotorsForTime(5000);

            //And we change the direction again :) 
            MotorCtrl.waitTimeLeft = MotorCtrl.PulseMs.ms2;
            MotorCtrl.waitTimeRight = MotorCtrl.PulseMs.ms1;
            MotorCtrl.MoveMotorsForTime(5000);

        }

        
    }
}
