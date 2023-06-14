using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LR1.Entities;
using LR1.Model;

namespace LR1.Model
{
    public class ListTitle:ObservableCollection<Title>
    {
        public ListTitle()
        {
            var queryTitle = from title in Workers.DataEntitiesEmployee.Titles select title;
            foreach (Title titl in queryTitle)
            {
                this.Add(titl);
            }
        }
    }
}
