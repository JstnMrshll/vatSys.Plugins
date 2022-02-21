# VatSys.Plugins
Plugins for VatSys for use with VatSim

# FDR Enhancer
Supplements the data within an FDR data record. Currently it provides:
- Airline callsign in the local label when no runway is selected
- Destination airport name and first waypoint in local label after runway has been selected

# Liftoff Timer
Provides a timer label in the each aircraft's track tags to indicate how long it has been since the aircraft took off.

Download the DLL from the Releases area and place it in the Plugins folder for vatSys (typically C:\Program Files (x86)\vatSys\bin\Plugins).
You will also need to either download the custom Labels.xml from above or add custom labels where you want the timer to appear in your existing Labels.xml. The label tag needed is "TIME_SINCE_LIFTOFF".
