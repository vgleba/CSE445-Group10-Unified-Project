<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="WebApplication1.Login" %>
<!DOCTYPE html>
<html>
<head runat="server"><title>Login</title></head>
<body>
<form id="form1" runat="server">
    <h2>Login</h2>
    <div>
        <label>Username:</label>
        <asp:TextBox runat="server" ID="txtUser" />
    </div>
    <div>
        <label>Password:</label>
        <asp:TextBox runat="server" ID="txtPass" TextMode="Password" />
    </div>
    <div>
        <asp:Button runat="server" ID="btnLogin" Text="Login" OnClick="btnLogin_Click" />
        <asp:Button runat="server" ID="btnBackToDefault" Text="Back to Default page" OnClick="btnBackToDefault_Click" />
    </div>
    <div>
        <asp:Label runat="server" ID="lblError" ForeColor="Red" />
    </div>
    <div>
        <asp:HyperLink ID="hlRegister" runat="server"
                      NavigateUrl="~/Register.aspx"
                      Text="New user? Register as a member" />
    </div>
</form>
</body>
</html>