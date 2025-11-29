<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Top10_TryIt.aspx.cs"
    Inherits="AAI_WebApp.TryIt.Top10_TryIt" %>
<!DOCTYPE html>
<html>
<head runat="server">
    <title>Top10ContentWords TryIt</title>
</head>
<body>
<form id="form1" runat="server">
    <h1>Top10ContentWords – TryIt</h1>
    <p>Enter text or a URL:</p>
    <asp:TextBox ID="txtInput" runat="server" TextMode="MultiLine"
                 Rows="6" Columns="80" />
    <br />
    <asp:Button ID="btnCall" runat="server" Text="Call Service" OnClick="btnCall_OnClick" />
    <br /><br />
    <asp:BulletedList ID="bltResults" runat="server" />
</form>
</body>
</html>
