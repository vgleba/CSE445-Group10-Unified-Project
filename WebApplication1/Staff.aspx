<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Staff.aspx.cs" Inherits="WebApplication1.Staff" %>
<!DOCTYPE html>
<html>
<head runat="server">
    <title>Staff Area</title>
    <style>
        .validation-error { color: red; font-size: 12px; display: block; margin-top: 2px; }
        .tryit-section { margin-bottom: 30px; border: 1px solid #ccc; padding: 15px; }
        .input-group { margin-bottom: 10px; }
        .input-group label { display: block; font-weight: bold; margin-bottom: 3px; }
    </style>
</head>
<body>
<form id="form1" runat="server">
    <h1>Staff Area</h1>
    <asp:Label ID="lblWelcome" runat="server" />
    <br />
    <asp:Button runat="server" ID="btnLogout" Text="Logout" OnClick="btnLogout_Click" />

    <hr />

    <h2>Change Password</h2>
    <asp:Label ID="lblChangePasswordMessage" runat="server" ForeColor="Red" />
    <div class="input-group">
        <label>Current Password:</label>
        <asp:TextBox ID="txtOldPassword" runat="server" TextMode="Password" />
    </div>
    <div class="input-group">
        <label>New Password:</label>
        <asp:TextBox ID="txtNewPassword" runat="server" TextMode="Password" />
    </div>
    <div class="input-group">
        <label>Confirm New Password:</label>
        <asp:TextBox ID="txtConfirmNewPassword" runat="server" TextMode="Password" />
    </div>
    <div>
        <asp:Button ID="btnChangePassword" runat="server"
                   Text="Change Password"
                   OnClick="btnChangePassword_Click" />
    </div>

    <hr />

    <h2>Staff Services Directory</h2>
    <asp:GridView ID="gvStaffDirectory" runat="server" AutoGenerateColumns="false">
        <Columns>
            <asp:BoundField HeaderText="Provider" DataField="Provider" />
            <asp:BoundField HeaderText="Type" DataField="Type" />
            <asp:BoundField HeaderText="Name" DataField="Name" />
            <asp:BoundField HeaderText="Parameters" DataField="Parameters" />
            <asp:BoundField HeaderText="Return" DataField="Return" />
            <asp:BoundField HeaderText="Description" DataField="Description" />
            <asp:TemplateField HeaderText="TryIt">
                <ItemTemplate>
                    <a href='<%# Eval("TryItUrl") %>'>Try It</a>
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>

    <hr />

    <h2>TryIt - Staff Services</h2>

    <!-- Catalog Add -->
    <div id="tryitCatalogAdd" class="tryit-section">
        <h3>Catalog Add (REST)</h3>
        <div class="input-group">
            <label>Category:</label>
            <asp:TextBox runat="server" ID="txtCategoryAdd" Width="200" />
            <asp:RequiredFieldValidator runat="server" ControlToValidate="txtCategoryAdd" 
                ErrorMessage="Category is required" ValidationGroup="CatalogAdd" 
                CssClass="validation-error" Display="Dynamic" />
        </div>
        <div class="input-group">
            <label>Item:</label>
            <asp:TextBox runat="server" ID="txtItemAdd" Width="200" />
            <asp:RequiredFieldValidator runat="server" ControlToValidate="txtItemAdd" 
                ErrorMessage="Item is required" ValidationGroup="CatalogAdd" 
                CssClass="validation-error" Display="Dynamic" />
        </div>
        <asp:Button runat="server" ID="btnCatalogAdd" Text="Add" OnClick="btnCatalogAdd_Click" ValidationGroup="CatalogAdd" />
        <pre><asp:Literal runat="server" ID="litCatalogAddResult" /></pre>
    </div>

    <!-- Catalog Delete -->
    <div id="tryitCatalogDelete" class="tryit-section">
        <h3>Catalog Delete (REST)</h3>
        <div class="input-group">
            <label>Category:</label>
            <asp:TextBox runat="server" ID="txtCategoryDel" Width="200" />
            <asp:RequiredFieldValidator runat="server" ControlToValidate="txtCategoryDel" 
                ErrorMessage="Category is required" ValidationGroup="CatalogDelete" 
                CssClass="validation-error" Display="Dynamic" />
        </div>
        <div class="input-group">
            <label>Item:</label>
            <asp:TextBox runat="server" ID="txtItemDel" Width="200" />
            <asp:RequiredFieldValidator runat="server" ControlToValidate="txtItemDel" 
                ErrorMessage="Item is required" ValidationGroup="CatalogDelete" 
                CssClass="validation-error" Display="Dynamic" />
        </div>
        <asp:Button runat="server" ID="btnCatalogDelete" Text="Delete" OnClick="btnCatalogDelete_Click" ValidationGroup="CatalogDelete" />
        <pre><asp:Literal runat="server" ID="litCatalogDeleteResult" /></pre>
    </div>

</form>
</body>
</html>