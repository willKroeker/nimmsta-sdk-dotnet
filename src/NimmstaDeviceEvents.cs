using Nimmsta.Net.CoreApi.Response;

namespace Nimmsta.Net
{
    public class NimmstaDeviceEvents(NimmstaDeviceApi deviceApi)
    {
        public bool CheckReceivedDeviceActionEvent(NimmstaEvent nimmstaEvent)
        {
            if (nimmstaEvent.DeviceAddress != deviceApi.DeviceAddress)
                return false;

            switch (nimmstaEvent.EventName)
            {
                case "ScanEvent":
                    ScanEventAction?.Invoke(nimmstaEvent);
                    return true;
                case "TouchEvent":
                    TouchEventAction?.Invoke(nimmstaEvent);
                    return true;
                case "ButtonEvent":
                    ButtonEventAction?.Invoke(nimmstaEvent);
                    return true;
                case "TriggerEvent":
                    TriggerEventAction?.Invoke(nimmstaEvent);
                    return true;
                case "DoubleTriggerEvent":
                    DoubleTriggerEventAction?.Invoke(nimmstaEvent);
                    return true;
                case "TripleTriggerEvent":
                    TripleTriggerEventAction?.Invoke(nimmstaEvent);
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Scan events happen when a user scans a barcode with the NIMMSTA Smart Watch.
        /// The event contains the scanned barcode string after rules are applied, if there are any.
        /// Please be aware that barcodes are interpreted as WINDOWS-1252 by default.
        /// If you need another encoding, you have to decode the base64 string using the encoding your barcode uses.
        /// <br/>
        /// <br><b>barcode</b> (string): Scanned barcode.</br>
        /// <br><b>barcodeBytes</b> (string): Base64 encoded barcode(can be of any encoding)</br>
        /// </summary>
        /// <remarks>
        /// Response (sample):
        /// <code>
        /// {
        ///   "type": "EVENT",
        ///   "device": "E1:98:8B:41:2A:70",
        ///   "event": {
        ///     "name": "ScanEvent",
        ///     "data": {
        ///       "barcode": "test123"
        ///       "barcodeBytes": "dGVzdDEyMw=="
        ///     }
        ///   }
        /// }
        /// </code>
        /// </remarks>

        public Action<NimmstaEvent>? ScanEventAction { get; set; }

        /// <summary>
        /// Touch events occur if a user touches the display of the NIMMSTA Smart Watch.
        /// The event contains the x and y coordinates of the touch location.
        /// <br/>
        /// <br><b>x</b> (number): X coordinate on the screen.</br>
        /// <br><b>y</b> (number): Y coordinate on the screen.</br>
        /// </summary>
        /// <remarks>
        /// Response (sample):
        /// <code>
        /// {
        ///   "type": "EVENT",
        ///   "device": "E1:98:8B:41:2A:70",
        ///   "event": {
        ///     "name": "TouchEvent",
        ///     "data": {
        ///       "x": 0.0,
        ///       "y": 0.0
        ///     }
        ///   }
        /// }
        /// </code>
        /// </remarks>
        public Action<NimmstaEvent>? TouchEventAction { get; set; }

        /// <summary>
        /// Button events occur if a user touches a button on the display of the NIMMSTA Smart Watch.
        /// The event contains the title of the button.
        /// <br/>
        /// <br><b>value</b> (string): Name of the button that was clicked.</br>
        /// </summary>
        /// <remarks>
        /// Response (sample):
        /// <code>
        /// {
        ///   "type": "EVENT",
        ///   "device": "E1:98:8B:41:2A:70",
        ///   "event": {
        ///     "name": "ButtonEvent",
        ///     "data": {
        ///       "value": "button1"
        ///     }
        ///   }
        /// }
        /// </code>
        /// </remarks>
        public Action<NimmstaEvent>? ButtonEventAction { get; set; }

        /// <summary>
        /// Trigger Events occur when the user presses the trigger on the device
        /// (the trigger is the button used to scan barcodes).
        /// </summary>
        public Action<NimmstaEvent>? TriggerEventAction { get; set; }

        /// <summary>
        /// Double Trigger Events occur when the user presses the trigger on
        /// the device twice in quick succession
        /// (the trigger is the button used to scan barcodes).
        /// </summary>
        /// <remarks>
        /// Before receiving a DoubleTriggerEvent, you will always receive a TriggerEvent.
        /// </remarks>
        public Action<NimmstaEvent>? DoubleTriggerEventAction { get; set; }

        /// <summary>
        /// Triple Trigger Events occur when the user presses the trigger on
        /// the device three times in quick succession
        /// (the trigger is the button used to scan barcodes).
        /// </summary>
        /// <remarks>
        /// Before receiving a TripleTriggerEvent, you will always receive first
        /// a TriggerEvent and then a DoubleTriggerEvent.
        /// </remarks>
        public Action<NimmstaEvent>? TripleTriggerEventAction { get; set; }
    }
}
