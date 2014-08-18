using MvcSiteMapProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TalismanSqlForum.Code
{
    public class MenuDynamicNodeProvider : DynamicNodeProviderBase
    {
        public override IEnumerable<DynamicNode> GetDynamicNodeCollection(ISiteMapNode node_)
        {
            using (TalismanSqlForum.Models.ApplicationDbContext _db = new Models.ApplicationDbContext())
            {
                var nodes = new List<DynamicNode>();
                var items = _db.tForumLists.ToList();
                foreach (var item in items)
                {
                    DynamicNode node = new DynamicNode();
                    // ключ должен быть уникальным для каждой ноды
                    node.Key = "ForumList_" + item.Id.ToString();
                    node.RouteValues.Add("id", item.Id);
                    node.Action = "Index";
                    node.Controller = "ForumThemes";
                    node.Title = item.tForumList_name;
                    nodes.Add(node);
                    if (item.tForumThemes != null)
                    {
                        DynamicNode c = new DynamicNode();
                        c.Key = "CreateForumThemes_" + item.Id.ToString(); ;
                        c.ParentKey = node.Key;
                        c.Action = "Create";
                        c.Controller = "ForumThemes";
                        c.RouteValues.Add("id", item.Id);
                        c.Title = "Создать новую тему";
                        nodes.Add(c);
                        foreach (var item2 in item.tForumThemes)
                        {
                            DynamicNode node2 = new DynamicNode();
                            node2.Key = "ForumMessages_" + item2.Id.ToString();
                            node2.ParentKey = node.Key;
                            node2.RouteValues.Add("id", item2.Id);
                            node2.RouteValues.Add("id_fl", item2.tForumList.Id);
                            node2.Action = "Index";
                            node2.Controller = "ForumMessages";
                            node2.Title = item2.tForumThemes_name;
                            nodes.Add(node2);
                        }
                    }
                }
                _db.Dispose();
                return nodes;
            }
        }
    }
}