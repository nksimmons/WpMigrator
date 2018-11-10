using HtmlAgilityPack;

namespace WpMigrator.Extensions
{
    public static class NodeExtensions
    {
        // Lifted from https://stackoverflow.com/questions/12787449/html-agility-pack-removing-unwanted-tags-without-removing-content
        public static int RemoveNodesButKeepChildren(this HtmlNode rootNode, string xPath)
        {
            var nodes = rootNode.SelectNodes(xPath);
            if (nodes == null)
                return 0;
            foreach (var node in nodes)
                node.RemoveButKeepChildren();
            return nodes.Count;
        }

        public static void RemoveButKeepChildren(this HtmlNode node)
        {
            foreach (var child in node.ChildNodes)
                node.ParentNode.InsertBefore(child, node);
            node.Remove();
        }
    }
}
