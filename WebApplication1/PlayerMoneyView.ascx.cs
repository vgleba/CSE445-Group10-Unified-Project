using System;
using System.Web.UI;

namespace WebApplication1
{
    public partial class PlayerMoneyView : UserControl
    {
        public Guid PlayerId { get; private set; }

        public int Stack { get; private set; }

        public int CurrentBet { get; private set; }

        public bool Folded { get; private set; }

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        public void BindPlayer(Guid playerId, int stack, int currentBet, bool folded)
        {
            PlayerId = playerId;
            Stack = stack;
            CurrentBet = currentBet;
            Folded = folded;

            litPlayerId.Text = playerId.ToString();
            litPlayerStack.Text = stack.ToString();
            litPlayerBet.Text = currentBet.ToString();
            litPlayerStatus.Text = folded ? "(folded)" : "active";
        }
    }
}
