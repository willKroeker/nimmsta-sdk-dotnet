using System.Xml.Linq;

namespace Nimmsta.Net.Layout;

/// <summary>
/// The Smart Watch screen has a resolution of 200px x 200px
/// and a diameter of 1.54 inches (3.91 cm). As this is not
/// a big screen you have to think about the most important
/// part of your application and how to save space.
/// <br>Another thing to keep in mind is that the
/// NIMMSTA Smart Watch has a whitepaper display.
/// While this saves power and therefore enables great battery life,
/// it also has a long update time.</br>
/// <br>It is normal for the screen to take roughly 0.5-1.0 seconds until it is refreshed.</br>
/// </summary>
/// <remarks>
/// Currently, the layout is evaluated on the client side
/// and then sent over to the NIMMSTA Smart Watch.
/// You should change the screen as rarely as possible
/// to improve the user-felt responsiveness.
/// <br/>The layout itself is based on a static layout system.
/// Every element is statically placed on the screen.
/// </remarks>
public class NimmstaLayout(string name)
{
    /// <summary>
    /// Name of the layout instance
    /// </summary>
    public string Name { get; } = name;

    /// <summary>
    /// Currently static value as documented here
    /// <see href="https://docs.nimmsta.com/layout/1Layouts/#boilerplate-and-screen-definition"/>
    /// </summary>
    public float DeviceWidthInch { get; } = 1.54f;

    /// <summary>
    /// Currently static value as documented here
    /// <see href="https://docs.nimmsta.com/layout/1Layouts/#boilerplate-and-screen-definition"/>
    /// </summary>
    public float DeviceHeightInch { get; } = 1.54f;

    /// <summary>
    /// Currently static value as documented here
    /// <see href="https://docs.nimmsta.com/layout/1Layouts/#boilerplate-and-screen-definition"/>
    /// </summary>
    public int DeviceWidthPx { get; } = 200;

    /// <summary>
    /// Currently static value as documented here
    /// <see href="https://docs.nimmsta.com/layout/1Layouts/#boilerplate-and-screen-definition"/>
    /// </summary>
    public int DeviceHeightPx { get; } = 200;

    public NimmstaLayoutScreen Screen { get; set; } = new NimmstaLayoutScreen();

    public XDocument CreateLayoutXml()
    {
        return new XDocument(new XDeclaration("1.0", "utf-8", null),
            new XElement("NimmstaLayout",
                new XAttribute("name", Name),
                new XElement("device",
                    new XAttribute("width", DeviceWidthInch),
                    new XAttribute("height", DeviceHeightInch),
                    new XAttribute("pxx", DeviceWidthPx),
                    new XAttribute("pxy", DeviceHeightPx),
                    Screen.CreateLayoutXml()
                )
            )
        );
    }

    public static implicit operator XDocument(NimmstaLayout nimmstaLayout)
        => nimmstaLayout.CreateLayoutXml();
}
