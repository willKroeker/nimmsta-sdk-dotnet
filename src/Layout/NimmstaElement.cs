using System.Xml.Linq;

namespace Nimmsta.Net.Layout;

/// <summary>
/// All NIMMSTA layout elements share some basic properties to position them.
/// </summary>
public abstract class NimmstaElement(string name)
{
    protected abstract string LayoutXmlElementName { get; }

    /// <summary>
    /// Name of the cell to find them within a layout or name of an icon
    /// </summary>
    public string Name { get; set; } = name;

    /// <summary>
    /// Text to be displayed
    /// </summary>
    public string? Value { get; set; }

    /// <summary>
    /// When x is null, NIMMSTA will assign 0
    /// </summary>
    public int? X { get; set; }

    /// <summary>
    /// When y is null, it will assign the previous sibling
    /// view's y + the previous sibling view's height
    /// </summary>
    public int? Y { get; set; }

    /// <summary>
    /// When width is null, it will assign the width of the screen - x
    /// </summary>
    public int? Width { get; set; }

    /// <summary>
    /// When height is null, it will assign the intrinsic height
    /// of the view, e.g. for text this is equal to the line height.
    /// </summary>
    public int? Height { get; set; }

    public virtual XElement CreateLayoutXml()
    {
        var xmlElement = new XElement(LayoutXmlElementName);

        if (!string.IsNullOrEmpty(Name))
            xmlElement.Add(new XAttribute("name", Name));

        if (X != null)
            xmlElement.Add(new XAttribute("x", X.Value));
        if (Y != null)
            xmlElement.Add(new XAttribute("y", Y.Value));

        if (Width != null)
            xmlElement.Add(new XAttribute("width", Width.Value));
        if (Height != null)
            xmlElement.Add(new XAttribute("height", Height.Value));

        xmlElement.Add(Value);

        return xmlElement;
    }
}
