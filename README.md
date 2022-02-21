# vatSys.Plugins
Plugins for vatSys for use with VatSim

# Liftoff Timer
Provides a timer label in the each aircraft's track tags to indicate how long it has been since the aircraft lifted off.  
The timer will disappear after the following times:
- Medium aircraft : 2 minutes
- Heavy/Super Heavy aircraft : 3 minutes
- Unknown wake type : 3 minutes

![image](https://user-images.githubusercontent.com/35731217/154901022-4e9aab5f-b8c5-48d2-8357-1ae64a8e3642.png)   ![image](https://user-images.githubusercontent.com/35731217/154906458-038fcd71-69fb-4abc-b5b4-4a5360fdf834.png)


Download the DLL from the [Releases](https://github.com/JstnMrshll/vatSys.Plugins/releases) area and place it in the Plugins folder for vatSys (typically C:\Program Files (x86)\vatSys\bin\Plugins).  
You will also need to either download the custom [Labels.xml](https://github.com/JstnMrshll/vatSys.Plugins/blob/master/Supporting%20Files/Labels.xml) from the [Supporting Files](https://github.com/JstnMrshll/vatSys.Plugins/tree/master/Supporting%20Files) folder above or add custom labels where you want the timer to appear in your existing Labels.xml. The label tag needed is "TIME_SINCE_LIFTOFF". The Labels file is located in C:\Users\<USERNAME>\Documents\vatSys Files\Profiles\Australia

# FDR Enhancer (to be released soon)
Supplements the data within an FDR data record. Currently it provides:
- Destination Airport and Airline callsign in the local label when no runway is selected or it is past the PREA state.
- Destination airport name and first waypoint in local label after runway has been selected and it is in a PREA state. This allows easier provision of clearances without having to open up the flight plan details.

![image](https://user-images.githubusercontent.com/35731217/154904387-31c59b29-863c-4bd3-a04e-f3a1a2245323.png)

Coming features:
- Validate RFL & CFL based on direction of flight.
- Add global tag if flight plan lodged with TXT or Receive Only flags for easier recognition of special requirements.
