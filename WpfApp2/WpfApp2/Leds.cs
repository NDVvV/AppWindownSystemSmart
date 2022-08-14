using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfAppWindownSystemSmart
{
    internal class Leds
    {
        int value;
        public event EventHandler Changed;
        public int Value
        {
            get => value;
            set
            {
                if (this.value != value)
                {
                    this.value = value;
                    Changed?.Invoke(this, EventArgs.Empty);
                }
            }
        }
    }
}
