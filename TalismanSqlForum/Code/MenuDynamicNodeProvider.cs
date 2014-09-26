using MvcSiteMapProvider;
using System.Collections.Generic;
using System.Linq;

namespace TalismanSqlForum.Code
{
    public class MenuDynamicNodeProvider : DynamicNodeProviderBase
    {
        public override IEnumerable<DynamicNode> GetDynamicNodeCollection(ISiteMapNode node_)
        {
            using (var db = new Models.ApplicationDbContext())
            {
                var nodes = new List<DynamicNode>();
                var items = db.tForumLists.ToList();
                foreach (var item in items)
                {
                    var node = new DynamicNode {Key = "ForumList_" + item.Id.ToString()};
                    // ключ должен быть уникальным для каждой ноды
                    node.RouteValues.Add("id", item.Id);
                    node.Action = "Index";
                    node.Controller = "ForumThemes";
                    node.Title = item.tForumList_name;
                    nodes.Add(node);
                    if (item.tForumThemes == null) continue;
                    var c = new DynamicNode
                    {
                        Key = "CreateForumThemes_" + item.Id.ToString(),
                        ParentKey = node.Key,
                        Action = "Create",
                        Controller = "ForumThemes",
                        Title = "Создать новую тему"
                    };
                    c.RouteValues.Add("id", item.Id);
                    nodes.Add(c);
                    foreach (var item2 in item.tForumThemes)
                    {
                        var node2 = new DynamicNode
                        {
                            Key = "ForumMessages_" + item2.Id.ToString(), 
                            ParentKey = node.Key
                        };
                        node2.RouteValues.Add("id", item2.Id);
                        node2.RouteValues.Add("id_list", item2.tForumList.Id);
                        node2.Action = "Index";
                        node2.Controller = "ForumMessages";
                        if (item2.tForumThemes_name.Length > 30)
                        {
                            node2.Title = item2.tForumThemes_name.Substring(0,30)+"...";
                        }
                        else
                        {
                            node2.Title = item2.tForumThemes_name;
                        }
                        nodes.Add(node2);
                    }
                }
                db.Dispose();
                return nodes;
            }
        }
    }
}