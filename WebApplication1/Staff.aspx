<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Staff.aspx.cs" Inherits="WebApplication1.Staff" %>
<!DOCTYPE html>
<html>
<head runat="server"><title>Staff Area</title></head>
<body>
<form id="form1" runat="server">
    <h1>Staff Area</h1>
    <asp:Label ID="lblWelcome" runat="server" />
    <p>Welcome to the staff area. This will be fully implemented in Assignment 6.</p>
    <asp:Button runat="server" ID="btnLogout" Text="Logout" OnClick="btnLogout_Click" />

    <h2>Change Password</h2>
    <p>Use this form to change the password for your staff account.</p>

    <asp:Label ID="lblChangePasswordMessage" runat="server" ForeColor="Red" />

    <div>
        <label>Current Password:</label>
        <asp:TextBox ID="txtOldPassword" runat="server" TextMode="Password" />
    </div>
    <div>
        <label>New Password:</label>
        <asp:TextBox ID="txtNewPassword" runat="server" TextMode="Password" />
    </div>
    <div>
        <label>Confirm New Password:</label>
        <asp:TextBox ID="txtConfirmNewPassword" runat="server" TextMode="Password" />
    </div>
    <div>
        <asp:Button ID="btnChangePassword" runat="server"
                   Text="Change Password"
                   OnClick="btnChangePassword_Click" />
    </div>
</form>
</body>
</html>