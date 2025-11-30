<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Register.aspx.cs" Inherits="WebApplication1.Register" %>
<!DOCTYPE html>
<html>
<head runat="server"><title>Register</title></head>
<body>
<form id="form1" runat="server">
    <h2>Register New Member Account</h2>
    <div>
        <label>Username:</label>
        <asp:TextBox runat="server" ID="txtUsername" />
    </div>
    <div>
        <label>Password:</label>
        <asp:TextBox runat="server" ID="txtPassword" TextMode="Password" />
    </div>
    <div>
        <label>Confirm Password:</label>
        <asp:TextBox runat="server" ID="txtConfirmPassword" TextMode="Password" />
    </div>
    <div>
        <asp:Button runat="server" ID="btnRegister" Text="Register" OnClick="btnRegister_Click" />
    </div>
    <div>
        <asp:Literal runat="server" ID="litMessage" />
    </div>
    <div style="margin-top: 10px;">
        <asp:HyperLink ID="hlLogin" runat="server"
                      NavigateUrl="~/Login.aspx"
                      Text="Already have an account? Login" />
    </div>
</form>
</body>
</html>
