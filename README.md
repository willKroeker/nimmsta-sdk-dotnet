# Welcome to the Nimmsta.Net project
Open Source NIMMSTA SDK repository for .NET applications.

# Getting started

tbd.

## Samples
Usage of connection API:
```cs
var nimmstaWebSocketClient = new NimmstaWebSocketClient("localhost", 64693);
var nimmstaConnection = new NimmstaConnectionApi(nimmstaWebSocketClient);

// var connectedDevices = await nimmstaConnection.GetDevicesAsync();

var nimmstaDevice = new NimmstaDeviceApi(nimmstaWebSocketClient, "deviceaddress");
// alt
nimmstaConnection.DeviceConnected = async (newNimmstaDevice, nimmstaEvent) =>
{
    var devAdr = nimmstaEvent.DeviceAddress; // just to show how device address can be determined
    // ... send initial layout to device, etc.
};

```

Usage of layout:
```cs
    public NimmstaLayout BuildLayout(JsonObject layoutData)
    {
        var nimmstaLayout = new NimmstaLayout("MyLogicId");

        nimmstaLayout.Screen.StaticElements.Add(new NimmstaStatusBar());
        nimmstaLayout.Screen.StaticElements.Add(new NimmstaCell("logicTitle") {
            Value = "Title text",
            FontSize = NimmstaFontSize.Font11pt,
            HorizontalAlignment = NimmstaTextAlignment.CENTER,
        });
        nimmstaLayout.Screen.StaticElements.Add(new NimmstaHorizontalLine());

        // you dynamic child control processing if needed
        //foreach (var nimmstaLayoutControl in ChildControls.OfType<INimmstaLayoutControl>())
        //    nimmstaLayoutControl.BuildLayout(nimmstaLayout.Screen.StaticElements, layoutData);

        nimmstaLayout.Screen.StaticElements.Add(new NimmstaScanResult() {
            TimeToShowBarcode = 10,
            HorizontalAlignment = NimmstaTextAlignment.CENTER,
            VerticalAlignment = NimmstaTextAlignment.END,
        });

        // make all left alligned (if needed):
        nimmstaLayout.Screen.StaticElements
            .OfType<NimmstaCell>()
            .Where(nc => nc.HorizontalAlignment == NimmstaTextAlignment.START
                        && nc.X == 0)
            .ForEachAction(nc => { nc.X = 3; });

        return nimmstaLayout;
    }

    public void CheckAddUpdatedLayoutData(JsonObject layoutData)
    {
        foreach (var nimmstaLayoutControl in ChildControls.OfType<INimmstaLayoutControl>())
            nimmstaLayoutControl.CheckAddUpdatedLayoutData(layoutData);
    }
```



# Build

tbd.

# Testing

tbd.

# Contributing
This project is my personal code and it will not be supported by NIMMSTA itself,
even if they are aware of this project. Use it on you own risk, as can not guarantee
support for bugs. Of course you are invited to contribute and suggest changes or simply
create pull requests with suggested changes.

# License
Licensed under MIT license.

