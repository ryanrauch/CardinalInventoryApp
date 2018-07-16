using System;
using System.Collections.Generic;
using System.Text;

namespace CardinalInventoryApp.ViewModels
{
    public class StockItemLevelViewModel
    {
        private Decimal _itemLevel { get; set; }

        public StockItemLevelViewModel(Decimal level)
        {
            _itemLevel = level;
        }

        public string LevelText => String.Format("{0:P0}", _itemLevel);
        public string FALevelText => "\uf242";
    }
}
