using JumpKing.PauseMenu.BT.Actions;
using System;

namespace .Menu
{
    public class  : IOptions
    {
        public ()
            : base(Enum.GetNames(typeof()).Length, (int), EdgeMode.Wrap)
        {
        }

        protected override bool CanChange() => ;

        protected override string CurrentOptionName() => (()CurrentOption).ToString();

        protected override void OnOptionChange(int option)
        {
        }
    }
}
