<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Hash_TryIt.aspx.cs" Inherits="WebApplication1.TryIt.Hash_TryIt" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>Hash Function – TryIt</title>
</head>
<body>
    <form id="form1" runat="server">
        <h1>Hash Function – TryIt (Local DLL)</h1>

        <p>Enter a string to hash with SHA-256:</p>
        <asp:TextBox ID="txtInput" runat="server" Width="400" />

        <br />
        <asp:Button ID="btnHash" runat="server"
                    Text="Compute Hash"
                    OnClick="btnHash_Click" />

        <br /><br />
        <asp:Label ID="lblResultLabel" runat="server" Text="Hash:" />
        <br />
        <asp:TextBox ID="txtHash" runat="server"
                     TextMode="MultiLine"
                     Rows="4" Columns="80"
                     ReadOnly="true" />
    </form>
</body>
</html>
