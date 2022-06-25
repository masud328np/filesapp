using System;
using System.Collections.Generic;
using System.Linq;
using core.interfaces;
using core.models;

namespace web.Models
{
    public class TreeNode
    {
        private IFsEntity _entity;
        public TreeNode(IFsEntity entity)
        {
            _entity = entity;
        }

        public bool HasChildren => _entity.Children != null && _entity.Children.Any();
       
        public Guid Id => _entity.Id;
        // public IList<TreeNode> Children =>
        // _entity.Children.Select(x => new TreeNode(x)).ToList();
        
        public string Name => _entity.Name;
    }
}