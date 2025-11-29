<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="WebApplication1.Default" %>
<%@ Register Src="~/PlayerDeckView.ascx" TagPrefix="uc" TagName="PlayerDeckView" %>
<%@ Register Src="~/PlayerMoneyView.ascx" TagPrefix="uc" TagName="PlayerMoneyView" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>Assignment 5 - Service Directory & TryIt</title>
    <meta charset="utf-8" />
    <style>
        .validation-error { color: red; font-size: 12px; display: block; margin-top: 2px; }
        .tryit-section { margin-bottom: 30px; border: 1px solid #ccc; padding: 15px; }
        .input-group { margin-bottom: 10px; }
        .input-group label { display: block; font-weight: bold; margin-bottom: 3px; }
    </style>
</head>
<body>
<form id="form1" runat="server">

    <!-- Intro -->
    <section>
        <h1>Assignment 5 Web Application</h1>
    </section>

    <!-- Member / Staff -->
    <section>
        <asp:Button runat="server" ID="btnMember" Text="Member" OnClick="btnMember_Click" />
        <asp:Button runat="server" ID="btnStaff" Text="Staff" OnClick="btnStaff_Click" />
    </section>

    <!-- Service Directory -->
    <section>
        <h2>Service Directory</h2>
        <asp:GridView runat="server" ID="gvDirectory" AutoGenerateColumns="False" GridLines="Both">
            <Columns>
                <asp:BoundField DataField="Provider" HeaderText="Provider" />
                <asp:BoundField DataField="ComponentType" HeaderText="Component Type" />
                <asp:BoundField DataField="Operation" HeaderText="Operation" />
                <asp:BoundField DataField="Parameters" HeaderText="Parameters" />
                <asp:BoundField DataField="ReturnType" HeaderText="Return Type" />
                <asp:BoundField DataField="Description" HeaderText="Description" />
                <asp:TemplateField HeaderText="TryIt">
                    <ItemTemplate>
                        <asp:HyperLink runat="server" NavigateUrl='<%# Eval("TryItAnchor") %>' Text="Go" />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </section>

    <!-- TryIt Panels -->
    <section id="tryit">
        <h2>TryIt</h2>

        <!-- WCF: NewGame -->
        <div id="tryitPokerNewGame" class="tryit-section">
            <h3>PockerEngine New Game (REST)</h3>
            <asp:Button runat="server" ID="btnNewGame" Text="New Game" OnClick="btnNewGame_Click" />
            <pre><asp:Literal runat="server" ID="litPoker" /></pre>
        </div>

        <!-- REST: Apply poker action -->
        <div id="tryitPokerApplyAction" class="tryit-section">
            <h3>PockerEngine apply action (REST)</h3>
            <div class="input-group">
                <label>Game Id:</label>
                <asp:TextBox runat="server" ID="txtPokerGameId" Width="400" />
                <asp:RequiredFieldValidator runat="server" ControlToValidate="txtPokerGameId"
                    ErrorMessage="Game Id is required" ValidationGroup="PokerAction"
                    CssClass="validation-error" Display="Dynamic" />
            </div>
            <div class="input-group">
                <label>Action Type (Raise/Call/Fold):</label>
                <asp:TextBox runat="server" ID="txtPokerActionType" Width="200" />
                <asp:RequiredFieldValidator runat="server" ControlToValidate="txtPokerActionType"
                    ErrorMessage="Action type is required" ValidationGroup="PokerAction"
                    CssClass="validation-error" Display="Dynamic" />
            </div>
            <div class="input-group">
                <label>Amount (0 for fold/check):</label>
                <asp:TextBox runat="server" ID="txtPokerAmount" Width="200" />
            </div>
            <asp:Button runat="server" ID="btnPokerApplyAction" Text="Apply Action" OnClick="btnPokerApplyAction_Click" ValidationGroup="PokerAction" />
            <pre><asp:Literal runat="server" ID="litPokerApplyActionResult" /></pre>
        </div>

        <!-- REST: Apply poker action -->
        <div id="tryitPokerBot" class="tryit-section">
            <h3>PockerEngine Bot (WCF)</h3>
            <div class="input-group">
                <label>Game Id:</label>
                <asp:TextBox runat="server" ID="txtPokerBotGameId" Width="400" />
                <asp:RequiredFieldValidator runat="server" ControlToValidate="txtPokerBotGameId"
                    ErrorMessage="Game Id is required" ValidationGroup="PokerBot"
                    CssClass="validation-error" Display="Dynamic" />
            </div>
            <asp:Button runat="server" ID="btnPokerBot" Text="Ask Bot" OnClick="btnPokerBot_Click" ValidationGroup="PokerBot" />
            <pre><asp:Literal runat="server" ID="litPokerBotResult" /></pre>
        </div>

        <!-- Poker: Current Player View -->
        <div id="pokerDeckView" class="tryit-section">
            <h3>Current Player Deck View</h3>
            <div class="input-group">
                <label>Game Id:</label>
                <asp:TextBox runat="server" ID="txtPokerVisualizeGameId" Width="400" />
                <asp:RequiredFieldValidator runat="server" ControlToValidate="txtPokerVisualizeGameId"
                    ErrorMessage="Game Id is required" ValidationGroup="PokerVisualize"
                    CssClass="validation-error" Display="Dynamic" />
            </div>
            <asp:Button runat="server" ID="btnPokerDeckVisualize" Text="Visualize" OnClick="btnPokerDeckVisualize_Click" ValidationGroup="PokerVisualize" />
            <uc:PlayerDeckView runat="server" ID="playerDeckView" Visible="false" />
        </div>

        <!-- Poker: Players Money View -->
        <div id="pokerPlayersMoneyView" class="tryit-section">
            <h3>Players Money View</h3>
            <div class="input-group">
                <label>Game Id:</label>
                <asp:TextBox runat="server" ID="txtPokerMoneyGameId" Width="400" />
                <asp:RequiredFieldValidator runat="server" ControlToValidate="txtPokerMoneyGameId"
                    ErrorMessage="Game Id is required" ValidationGroup="PokerMoney"
                    CssClass="validation-error" Display="Dynamic" />
            </div>
            <asp:Button runat="server" ID="btnPokerMoneyVisualize" Text="Visualize" OnClick="btnPokerMoneyVisualize_Click" ValidationGroup="PokerMoney" />
            <asp:Literal runat="server" ID="litPokerMoneyStatus" />
            <asp:PlaceHolder runat="server" ID="phPlayersMoney" />

        </div>
        
        <!-- DLL Password hash -->
        <div id="tryitDllHash" class="tryit-section">
            <h3>Password hash (DLL)</h3>
            <div class="input-group">
                <label>Pass to hash:</label>
                <asp:TextBox runat="server" ID="txtDllHashInput" Width="400" />
                <asp:RequiredFieldValidator runat="server" ControlToValidate="txtDllHashInput" 
                    ErrorMessage="Data is required" ValidationGroup="DllHash" 
                    CssClass="validation-error" Display="Dynamic" />
            </div>
            <asp:Button runat="server" ID="btnDllHash" Text="Hash (DLL)" OnClick="btnDllHash_Click" ValidationGroup="DllHash" />
            <pre><asp:Literal runat="server" ID="litDllHashResult" /></pre>
        </div>

        <!-- DLL Password verify -->
        <div id="tryitDllVerify" class="tryit-section">
            <h3>Password verify (DLL)</h3>
            <div class="input-group">
                <label>Pass to verify:</label>
                <asp:TextBox runat="server" ID="txtDllVerifyInput" Width="400" />
                <asp:RequiredFieldValidator runat="server" ControlToValidate="txtDllVerifyInput" 
                    ErrorMessage="Data is required" ValidationGroup="DllVerify" 
                    CssClass="validation-error" Display="Dynamic" /> 
            </div>
            <div class="input-group">
                <label>Hashed pass:</label>
                <asp:TextBox runat="server" ID="txtDllHashedInput" Width="400" />
                <asp:RequiredFieldValidator runat="server" ControlToValidate="txtDllHashedInput" 
                    ErrorMessage="Data is required" ValidationGroup="DllVerify" 
                    CssClass="validation-error" Display="Dynamic" />
            </div>
            <asp:Button runat="server" ID="btnDllVerify" Text="Verify (DLL)" OnClick="btnDllVerify_Click" ValidationGroup="DllVerify" />
            <pre><asp:Literal runat="server" ID="litDllVerifyResult" /></pre>
        </div>
    </section>
</form>
</body>
</html>
