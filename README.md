# SimHub-Save
Modified from SimHubSDKDemo, save game data into csv file, add raw logitech G29 device data to property.

## csv file format
* First line: CurrentTime,SpeedKmh,Throttle,Brake,CurrentLap,CurrentLapTime,RawWheel,RawThrottle,RawBrake,Gear,TimeJumped
* Following line: data

## variable meanings
* **CurrentTime**: current time
* **SpeedKmh**: current speed (km/h)
* **Throttle**: throttle data in game, 0~100
* **Brake**: brake data in game, 0~100
* **CurrentLap**: current lap
* **CurrentLapTime**: time taken in current lap
* **RawWheel**: raw steering Wheel data from Logitech G29
* **RawThrottle**: raw throttle data from Logitech G29Cancel changes
* **RawBrake**: raw brake data from Logitech G29
* **Gear**: current gear
* **TimeJumped**: if current lap time jumped too much (>1s) from last update. Since recovery vehicle leads to penalty time, this can indicate whether the player goes off road
