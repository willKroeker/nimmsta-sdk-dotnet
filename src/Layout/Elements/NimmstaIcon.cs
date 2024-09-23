namespace Nimmsta.Net.Layout.Elements;

/// <summary>
/// Represents a specific icon on the screen of the NIMMSTA Smart Watch.
/// Currently the Icons provided are limitted to specific icons and sizes.
/// <br/>NIMMSTA docs: <see href="https://docs.nimmsta.com/layout/1Layouts/#icon"/>
/// <list type="bullet">
/// <item>error_cross of size 60x60px</item>
/// <item>success_tick of size 60x60px</item>
/// <item>nimmsta_logo of size 180x180px</item>
/// </list>
/// </summary>
public class NimmstaIcon(string name) : NimmstaElement(name)
{
    protected override string LayoutXmlElementName => "icon";

    public static NimmstaIcon CreateError()
        => new("error_cross");
    public static NimmstaIcon CreateSuccess()
        => new("success_tick");
    public static NimmstaIcon CreateNimmstaLogo()
        => new("nimmsta_logo");
}
