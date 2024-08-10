using Master_DAL.Premetives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Master_DAL.Models
{
    public class Modules : Entity
    {
        public Modules(): base(null) { }

        public Modules(
            string id,
            string name,
            string? role,
            string? targetUrl

            ) : base(id)
        {
            Name = name;
            Role = role;
            TargetUrl = targetUrl;
            SubModules = new List<SubModules>();
            
        }

        public string Name { get;set; }
        public string? Role { get;set; }
        public string? TargetUrl { get;set; }

        public ICollection<SubModules> SubModules { get; set; }
    }
}
