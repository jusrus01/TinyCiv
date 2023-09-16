using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace TinyCiv.Client.Code
{
    public class GameObject
    {
        public Border border;
        DateTime LastUpdate;
        TimeSpan TimeDelta;
        public int r;
        public int c;

        public GameObject(int r, int c) 
        { 
            this.r = r;
            this.c = c;
        }

        public void Start()
        {
            LastUpdate = DateTime.Now;
        }

        public void Update()
        {
            DateTime now = DateTime.Now;
            TimeDelta = now-LastUpdate;
            LastUpdate = now;
        }
    }
}
