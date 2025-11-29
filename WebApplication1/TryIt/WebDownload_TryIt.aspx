<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WebDownload_TryIt.aspx.cs" Inherits="WebApplication1.TryIt.WebDownload_TryIt" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>WebDownload – TryIt</title>
</head>
<body>
    <form id="form1" runat="server">
        <h1>WebDownload – TryIt</h1>

        <p>Enter a URL (http/https):</p>
        <asp:TextBox ID="txtUrl" runat="server" Width="400" />

        <br />
        <asp:Button ID="btnDownload" runat="server"
                    Text="Call WebDownload Service"
                    OnClick="btnDownload_Click" />

        <br /><br />
        <asp:Label ID="lblError" runat="server" ForeColor="Red" />

        <h2>Downloaded Text</h2>
        <asp:TextBox ID="txtResult" runat="server" TextMode="MultiLine"
                     Rows="15" Columns="100" ReadOnly="true" />
    </form>
</body>
</html>
