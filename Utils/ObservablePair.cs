using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetifePanel.Utils
{
    public partial class ObservablePair<TK, TV> : ObservableObject where TK : class where TV : class
    {
        [ObservableProperty]
        private TK key;

        [ObservableProperty]
        private TV value;

        public ObservablePair(TK key, TV value)
        {
            Key = key;
            Value = value;
        }
    }
}
