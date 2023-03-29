using GameReaderCommon;
using SimHub.Plugins;
using System;
using System.Windows.Media;

namespace User.PluginSdkDemo
{
    [PluginDescription("My plugin description")]
    [PluginAuthor("Author")]
    [PluginName("Demo plugin")]
    public class DataPluginDemo : IPlugin, IDataPlugin, IWPFSettingsV2
    {
        public DateTime CurrentTime;
        public TimeSpan Period;
        public System.IO.FileStream fs;
        public System.IO.StreamWriter sw;
        
        public double SpeedKmh;
        public double Throttle;
        public double Brake;
        public int CurrentLap;
        public string Gear;
        public TimeSpan CurrentLapTime;
        public TimeSpan LastLapTime;
        public int RawWheel;
        public int RawThrottle;
        public int RawBrake;

        public int TimeJumped;

        public DataPluginDemoSettings Settings;

        /// <summary>
        /// Instance of the current plugin manager
        /// </summary>
        public PluginManager PluginManager { get; set; }

        /// <summary>
        /// Gets the left menu icon. Icon must be 24x24 and compatible with black and white display.
        /// </summary>
        public ImageSource PictureIcon => this.ToIcon(Properties.Resources.sdkmenuicon);

        /// <summary>
        /// Gets a short plugin title to show in left menu. Return null if you want to use the title as defined in PluginName attribute.
        /// </summary>
        public string LeftMenuTitle => "Demo plugin";


        /// <summary>
        /// Called one time per game data update, contains all normalized game data,
        /// raw data are intentionnally "hidden" under a generic object type (A plugin SHOULD NOT USE IT)
        ///
        /// This method is on the critical path, it must execute as fast as possible and avoid throwing any error
        ///
        /// </summary>
        /// <param name="pluginManager"></param>
        /// <param name="data">Current game data, including current and previous data frame.</param>
        public void DataUpdate(PluginManager pluginManager, ref GameData data)
        {
            // Define the value of our property (declared in init)
            if (data.GameRunning)
            {
                if (data.OldData != null && data.NewData != null)
                {
                    CurrentTime = DateTime.Now;

                    SpeedKmh = data.OldData.SpeedKmh;
                    Throttle = data.OldData.Throttle;
                    Brake = data.OldData.Brake;
                    CurrentLap = data.OldData.CurrentLap;
                    CurrentLapTime = data.OldData.CurrentLapTime;
                    Gear = data.OldData.Gear;

                    RawWheel = Convert.ToInt32(PluginManager.GetPropertyValue("JoystickPlugin.Logitech_G_HUB_G29_Driving_Force_Racing_Wheel_USB_X")) - 32767; // raw steering wheel, -32767~32768
                    RawThrottle = 65535 - Convert.ToInt32(PluginManager.GetPropertyValue("JoystickPlugin.Logitech_G_HUB_G29_Driving_Force_Racing_Wheel_USB_Y")); // raw throttle, 0~65535
                    RawBrake = 65535 - Convert.ToInt32(PluginManager.GetPropertyValue("JoystickPlugin.Logitech_G_HUB_G29_Driving_Force_Racing_Wheel_USB_RZ")); // raw brake,0~65535

                    if ((CurrentLapTime - LastLapTime) >= Period) // if LapTime Jumped, mark recover device
                    {
                        // Trigger an event
                        TimeJumped = 1;
                        this.TriggerEvent("TimeJumped");
                    }
                    else TimeJumped = 0;

                    var line = string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10}", CurrentTime.ToString("hh:mm:ss:fff"), SpeedKmh, Throttle, Brake, CurrentLap, CurrentLapTime.TotalSeconds, RawWheel, RawThrottle, RawBrake, Gear, TimeJumped);
                    //CarDamagesAvg CarDamages1-5 CarDamagesMax 
                    // no: SpotterCarLeftAngle TurnIndicatorLeft
                    sw.WriteLine(line);
                    sw.Flush();


                    LastLapTime = CurrentLapTime;

                    //var Info = string.Format("SpeedKmh {0}, Throttle {1}, Brake {2}", data.OldData.SpeedKmh, data.OldData.Throttle, data.OldData.Brake);
                    //SimHub.Logging.Current.Info(Info);

                    /* update every 1s
                    if ((CurrentTime - LastUpdate) >= Period) //data.OldData.SpeedKmh >= Settings.SpeedWarningLevel
                    {
                        
                        // Trigger an event
                        // this.TriggerEvent("SpeedWarning");
                    } */
                }
            }
        }

        /// <summary>
        /// Called at plugin manager stop, close/dispose anything needed here !
        /// Plugins are rebuilt at game change
        /// </summary>
        /// <param name="pluginManager"></param>
        public void End(PluginManager pluginManager)
        {
            // Save settings
            this.SaveCommonSettings("GeneralSettings", Settings);
            sw.Close();

        }


        /// <summary>
        /// Returns the settings control, return null if no settings control is required
        /// </summary>
        /// <param name="pluginManager"></param>
        /// <returns></returns>
        public System.Windows.Controls.Control GetWPFSettingsControl(PluginManager pluginManager)
        {
            return new SettingsControlDemo(this);
        }

        /// <summary>
        /// Called once after plugins startup
        /// Plugins are rebuilt at game change
        /// </summary>
        /// <param name="pluginManager"></param>
        public void Init(PluginManager pluginManager)
        {
            SimHub.Logging.Current.Info("Starting demo plugin");

            // init time
            Period = new TimeSpan(0, 0, 1); //1s

            //init property value
            SpeedKmh = 0; Throttle = 0 ; Brake = 0; CurrentLap = 0; 
            CurrentLapTime = new TimeSpan(0, 0, 0, 0); LastLapTime = new TimeSpan(0, 0, 0, 0);
            Gear = "N";
            RawWheel = 0; RawThrottle = 0; RawBrake = 0;
            TimeJumped = 0;

            string filePath = @"C:\Users\zhaor\Desktop";  //file path
            string fileName = DateTime.Now.ToString("yyyy-MM-dd-hh-mm-ss-fff") + ".csv";
            filePath = System.IO.Path.Combine(filePath, fileName);

            System.IO.FileInfo fi = new System.IO.FileInfo(filePath);
            if (!fi.Directory.Exists)
            {
                fi.Directory.Create();  //create file
            }

            fs = new System.IO.FileStream(filePath, System.IO.FileMode.Create, System.IO.FileAccess.Write);
            sw = new System.IO.StreamWriter(fs, System.Text.Encoding.UTF8);
            string data = "CurrentTime,SpeedKmh,Throttle,Brake,CurrentLap,CurrentLapTime,RawWheel,RawThrottle,RawBrake,Gear,TimeJumped";
            sw.WriteLine(data);
            sw.Flush();
            
            // Load settings
            Settings = this.ReadCommonSettings<DataPluginDemoSettings>("GeneralSettings", () => new DataPluginDemoSettings());

            // Declare a property available in the property list, this gets evaluated "on demand" (when shown or used in formulas)
            //this.AttachDelegate("CurrentDateTime", () => DateTime.Now);
            //this.AttachDelegate("SpeedKmh", () => SpeedKmh);
            //this.AttachDelegate("Throttle", () => Throttle);
            //this.AttachDelegate("Brake", () => Brake);
            //this.AttachDelegate("CurrentLap", () => CurrentLap);
            //this.AttachDelegate("CurrentLapTime", () => CurrentLapTime);
            this.AttachDelegate("RawWheel", () => RawWheel);
            this.AttachDelegate("RawThrottle", () => RawThrottle);
            this.AttachDelegate("RawBrake", () => RawBrake);
            this.AttachDelegate("TimeJumped", () => TimeJumped);
            // SpeedKmh, Throttle, Brake, CurrentLap, CurrentLapTime, RawWheel, RawThrottle, RawBrake

            // add event
            this.AddEvent("TimeJumped");

            //add action
            this.AddAction("RecoverDevice", (a, b) =>
            {
                SimHub.Logging.Current.Info("Recover Device");
            });
        }
    }
}