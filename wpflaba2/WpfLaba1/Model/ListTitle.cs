using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfLaba1.Entities;

namespace WpfLaba1.Model
{
    class ListTitle : ObservableCollection<Title>
    {
        public ListTitle()
        {
            var queryTitle = from title in Sotrudniki.DataEntitiesEmployee.Titles select title;
            foreach (Title titl in queryTitle)
            {
                this.Add(titl);
            }
        }


    }
}
