using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace TinyCiv.Client.Code.BorderDecorators
{
    public abstract class BorderDecorator : IBorderObject
    {
        protected IBorderObject _decoratedObject;

        protected BorderDecorator(IBorderObject decoratedObject)
        {
            _decoratedObject = decoratedObject;
        }

        public virtual BorderProperties ApplyEffects()
        {
            return _decoratedObject.ApplyEffects();
        }

        public virtual BorderProperties RemoveEffects()
        {
            return _decoratedObject.RemoveEffects();
        }
    }
}
