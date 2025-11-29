<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ThreatNLP_TryIt.aspx.cs" Inherits="WebApplication1.TryIt.ThreatNLP_TryIt" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <meta charset="utf-8" />
    <title>ThreatNLP – TryIt</title>
</head>
<body>
    <form id="form1" runat="server">
        <h1>ThreatNLP – TryIt</h1>

        <p>
            <b>Endpoint:</b>
            <asp:Label ID="lblEndpoint" runat="server" />
        </p>

        <p>
            <b>Method:</b> POST <code>/api/threat/nlp</code><br />
            <b>Request:</b> <code>{ text: string, lang?: string, ts?: ISO-8601 }</code><br />
            <b>Response:</b> <code>ThreatEvent[]</code>
        </p>

        <h3>Input alert text</h3>
        <asp:TextBox ID="txtInput" runat="server" TextMode="MultiLine"
            Rows="6" Columns="80" />

        <br />
        <br />
        <asp:Button ID="btnExampleUA" runat="server" Text="Load UA example"
            OnClick="btnExampleUA_Click" />
        <asp:Button ID="btnExampleEN" runat="server" Text="Load EN example"
            OnClick="btnExampleEN_Click" />
        <asp:Button ID="btnClear" runat="server" Text="Clear"
            OnClick="btnClear_Click" />

        <br />
        <br />
        <h4>Cookie-based state (local component)</h4>
        <asp:Button ID="btnLoadFromCookie" runat="server"
            Text="Load last input from cookie"
            OnClick="btnLoadFromCookie_Click" />
        <br />
        <asp:Label ID="lblCookieInfo" runat="server" />

        <br />
        <br />
        <asp:Button ID="btnInvoke" runat="server" Text="Invoke ThreatNLP"
            OnClick="btnInvoke_Click" />

        <br />
        <br />
        <asp:Label ID="lblError" runat="server" ForeColor="Red" />

        <h3>Raw JSON response</h3>
        <asp:TextBox ID="txtRawJson" runat="server" TextMode="MultiLine"
            Rows="10" Columns="100" ReadOnly="true" />

        <h3>Parsed Events</h3>
        <asp:GridView ID="gvEvents" runat="server" AutoGenerateColumns="false">
            <Columns>
                <asp:BoundField HeaderText="Threat Type" DataField="ThreatType" />
                <asp:BoundField HeaderText="Direction (deg)" DataField="DirectionDeg" />
                <asp:BoundField HeaderText="Location" DataField="LocationText" />
                <asp:BoundField HeaderText="Timestamp" DataField="Timestamp" />
                <asp:BoundField HeaderText="Confidence" DataField="Confidence" />
            </Columns>
        </asp:GridView>
    </form>
</body>
</html>
