<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PlayerDeckView.ascx.cs" Inherits="WebApplication1.PlayerDeckView" %>
<div class="player-deck-view">
    <asp:Panel runat="server" ID="pnlContent" Visible="false">
        <pre>
+--------------------------------------------------------------+
|   ____                              Game: <asp:Literal runat="server" ID="litGameId" />
|  /    \   Board Cards               Stage: <asp:Literal runat="server" ID="litStage" />
| | (  ) |  [<asp:Literal runat="server" ID="litBoard" />]                   Pot: <asp:Literal runat="server" ID="litPot" />
|  \____/                             Current Bet: <asp:Literal runat="server" ID="litCurrentBet" />
|                                                              |
<asp:Literal runat="server" ID="litPlayers" />
+--------------------------------------------------------------+
        </pre>
    </asp:Panel>
    <asp:Panel runat="server" ID="pnlError" Visible="false">
        <asp:Literal runat="server" ID="litError" />
    </asp:Panel>
</div>
