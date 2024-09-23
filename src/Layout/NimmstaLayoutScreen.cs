using System.Xml.Linq;

namespace Nimmsta.Net.Layout;

public class NimmstaLayoutScreen(string name = "default")
{
    public string Name { get; } = name;

    public List<NimmstaElement> StaticElements { get; init; } = [];

    public XElement CreateLayoutXml()
    {
        return new XElement("screen",
            new XAttribute("default", "true"),
            new XAttribute("name", Name),
            new XElement("staticElements",
                StaticElements.Select(se => se.CreateLayoutXml())
            )
        );
    }

    public static implicit operator XElement(NimmstaLayoutScreen nimmstaLayoutScreen)
        => nimmstaLayoutScreen.CreateLayoutXml();
}
