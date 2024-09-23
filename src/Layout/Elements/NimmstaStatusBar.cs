namespace Nimmsta.Net.Layout.Elements;

/// <summary>
/// The standard status bar for devices. It consists of a connected indicator,
/// the battery-level, the battery icon and the charging icon.
/// By default, the status bar will be placed in the upper left-hand
/// corner with a height of 28px.
/// </summary>
/// <remarks>
/// Since Release 6.0, we also support custom text within the Status Bar.If not set, it will show the type of the imager.
/// </remarks>
public class NimmstaStatusBar() : NimmstaElement(string.Empty)
{
    protected override string LayoutXmlElementName => "statusBar";
}
