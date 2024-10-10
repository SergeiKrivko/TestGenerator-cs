using System.Xml;

namespace Shared.Utils;

public class SettingsFile : SettingsSection
{
    private Dictionary<string, SettingsSection> _sections;
    private string _path;

    private SettingsFile(string path, Dictionary<string, string?> global, 
        Dictionary<string, Dictionary<string, string?>> sections) : base("Global", global)
    {
        _path = path;
        _sections = new Dictionary<string, SettingsSection>();
        foreach (var element in sections)
        {
            var section = FromDictionary(element.Key, element.Value);
            if (section.Name != null)
                _sections[section.Name] = section;
        }
    }

    private static Dictionary<string, string?> ParseNode(XmlNode node)
    {
        var res = new Dictionary<string, string?>();
        foreach (var child in node.ChildNodes)
        {
            if (child is XmlNode)
            {
                var childNode = (XmlNode)child;
                res[childNode.Name] = childNode.InnerText;
            }
        }

        return res;
    }

    public static SettingsFile Open(string path)
    {
        Dictionary<string, string?> global = new();
        var sections = new Dictionary<string, Dictionary<string, string?>>();
        var document = new XmlDocument();
        try
        {
            document.Load(path);
            if (document.FirstChild != null)
            {
                var root = document.SelectSingleNode("settings");
                if (root != null)
                {
                    for (var i = 0; i < root.ChildNodes.Count; i++)
                    {
                        var node = root.ChildNodes[i];
                        if (node?.Name == "global")
                        {
                            global = ParseNode(node);
                        }
                        else
                        {
                            var attr = node?.Attributes?["name"];
                            if (node != null && attr != null)
                                sections.Add(attr.Value, ParseNode(node));
                        }
                    }
                }
            }
        }
        catch (FileNotFoundException)
        {
        }
        catch (XmlException)
        {
        }

        return new SettingsFile(path, global, sections);
    }
    
    
    private static XmlElement StoreNode(SettingsSection section, XmlDocument document)
    {
        var root = document.CreateElement("section");
        root.SetAttribute("name", section.Name);
        foreach (var item in section)
        {
            if (item.Value != null)
            {
                var tag = document.CreateElement(item.Key);
                tag.InnerText = item.Value;
                root.AppendChild(tag);
            }
        }

        return root;
    }

    private XmlElement StoreGlobal(XmlDocument document)
    {
        var root = document.CreateElement("global");
        foreach (var item in this)
        {
            if (item.Value != null)
            {
                var tag = document.CreateElement(item.Key);
                tag.InnerText = item.Value;
                root.AppendChild(tag);
            }
        }

        return root;
    }

    private void Store()
    {
        var document = new XmlDocument();
        var xmlDeclaration = document.CreateXmlDeclaration("1.0", "UTF-8", null);
        document.AppendChild(xmlDeclaration);

        var root = document.CreateElement("settings");
        document.AppendChild(root);

        foreach (var item in _sections)
        {
            root.AppendChild(StoreNode(item.Value, document));
        }

        root.AppendChild(StoreGlobal(document));

        document.Save(_path);
    }

    public SettingsSection GetSection(string name)
    {
        if (_sections.TryGetValue(name, out var section))
            return section;
        var newSection = Empty(name);
        _sections[name] = newSection;
        return newSection;
    }

    public bool DeleteSection(string name) => _sections.Remove(name);
}