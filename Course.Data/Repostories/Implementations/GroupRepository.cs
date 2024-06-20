using Course.Core.Entities;
using Course.Data.Repostories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Data.Repostories.Implementations
{
    public class GroupRepository:Repository<Group>, IGroupRepository
    {
        public GroupRepository(AppDbContext context) : base(context)
        {

        }
    }
}
