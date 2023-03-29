# SimHub-Save
Modified from SimHubSDKDemo, save game data into csv file, add raw logitech G29 device data to property.

## csv file format
* File name: current time, "yyyy-MM-dd-hh-mm-ss-fff.csv"
* First line: CurrentTime,SpeedKmh,Throttle,Brake,CurrentLap,CurrentLapTime,RawWheel,RawThrottle,RawBrake,Gear,TimeJumped
* Following line: data

## variable meanings
* ***CurrentTime***: current time, "hh:mm:ss:fff"
* ***SpeedKmh***: current speed (km/h)
* ***Throttle***: throttle data in game, 0~100
* ***Brake***: brake data in game, 0~100
* ***CurrentLap***: current lap
* ***CurrentLapTime***: time taken in current lap
* ***RawWheel***: steering Wheel data from Logitech G29, -32767~32768, 0 means no revolve
* ***RawThrottle***: throttle data from Logitech G29Cancel changes, 0~65535, 0 means no press
* ***RawBrake***: brake data from Logitech G29, 0~65535, 0 means no press
* ***Gear***: current gear
* ***TimeJumped***: if current lap time jumped too much (>1s) from last update. Since recovering vehicle leads to penalty time, this can indicate whether the player goes off road

# set up instruction
Replace DataPluginDemo.cs in folder "C:\Program Files (x86)\SimHub\PluginSdk\User.PluginSdkDemo" with the file in this project, open the .sln file and regenerate project. Then open SimHub, the csv file will appear on desktop (remember to modify the filepath in init()) and start recording data, and it will stop and be closed when SimHub is closed. 
