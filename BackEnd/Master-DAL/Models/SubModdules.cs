using Master_DAL.Premetives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Master_DAL.Models
{
    public class SubModules : Entity
    {
        public SubModules() : base(null) { }

        public SubModules(
            string id,
            string name,
            string? iconUrl,
            string? targetUrl,
            string? role,
            string moduleId,
            string rank
            ) : base(id)
        {
            Name = name;
            ModuleId = moduleId;
            Role = role;
            TargetUrl = targetUrl;
            Rank = rank;
            Menu = new List<Menu>();
            
        }
        public string Name { get;set; }
        public string? iconUrl { get;set; }
        public string? TargetUrl { get;set; }
        public string? Role { get;set; }
        public string Rank {  get;set; }
        public string ModuleId { get; set; }

        //Navigation Property
        public ICollection<Menu> Menu { get; set; }
        
        public Modules Modules { get; set; }
    }
}
