namespace WhereDidTheMoneyGo.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    public class CategoryViewModel
    {
        public IEnumerable<CategoryItemViewModel> Categories { get; set; }
    }
}
