<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Member.aspx.cs" Inherits="WebApplication1.Member" %>
<!DOCTYPE html>
<html>
<head runat="server">
    <title>Member Area</title>
    <style>
        .validation-error { color: red; font-size: 12px; display: block; margin-top: 2px; }
        .tryit-section { margin-bottom: 30px; border: 1px solid #ccc; padding: 15px; }
        .input-group { margin-bottom: 10px; }
        .input-group label { display: block; font-weight: bold; margin-bottom: 3px; }
    </style>
</head>
<body>
<form id="form1" runat="server">
    <h1>Member Area</h1>
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

    <h2>Member Services Directory</h2>
    <asp:GridView ID="gvMemberDirectory" runat="server" AutoGenerateColumns="false">
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

    <h2>TryIt - Member Services</h2>

    <!-- Catalog Get Item -->
    <div id="tryitCatalogGet" class="tryit-section">
        <h3>Catalog Get Item (REST)</h3>
        <div class="input-group">
            <label>Category:</label>
            <asp:TextBox runat="server" ID="txtCategoryGet" Width="200" />
            <asp:RequiredFieldValidator runat="server" ControlToValidate="txtCategoryGet" 
                ErrorMessage="Category is required" ValidationGroup="CatalogGet" 
                CssClass="validation-error" Display="Dynamic" />
        </div>
        <div class="input-group">
            <label>Item:</label>
            <asp:TextBox runat="server" ID="txtItemGet" Width="200" />
            <asp:RequiredFieldValidator runat="server" ControlToValidate="txtItemGet" 
                ErrorMessage="Item is required" ValidationGroup="CatalogGet" 
                CssClass="validation-error" Display="Dynamic" />
        </div>
        <asp:Button runat="server" ID="btnCatalogGet" Text="Get Item" OnClick="btnCatalogGet_Click" ValidationGroup="CatalogGet" />
        <asp:Button runat="server" ID="btnAddToCart" Text="Add to Cart" OnClick="btnAddToCart_Click" 
            style="margin-left: 10px;" Visible="false" />
        <pre><asp:Literal runat="server" ID="litCatalogGetResult" /></pre>
    </div>

    <!-- Cart -->
    <div id="tryitCart" class="tryit-section">
        <h3>Cart (REST)</h3>
        <div class="input-group">
            <label>Click to refresh and view cart contents:</label>
        </div>
        <asp:Button runat="server" ID="btnCartRefresh" Text="Refresh Cart" OnClick="btnCartRefresh_Click" />
        <asp:Button runat="server" ID="btnCartCheckout" Text="Checkout" OnClick="btnCartCheckout_Click" 
            style="margin-left: 10px;" />
        <pre><asp:Literal runat="server" ID="litCartResult" /></pre>
        
        <!-- Address Validation Panel -->
        <asp:Panel runat="server" ID="pnlAddress" Visible="false" style="margin-top: 20px; padding: 15px; border: 1px solid #ddd; background-color: #f9f9f9;">
            <h3>Shipping Address</h3>
            <div class="input-group">
                <asp:Label runat="server" Text="State:" AssociatedControlID="txtState" />
                <asp:TextBox runat="server" ID="txtState" Width="200" />
                <asp:RequiredFieldValidator runat="server" ControlToValidate="txtState" 
                    ErrorMessage="State is required" ValidationGroup="Address" 
                    CssClass="validation-error" Display="Dynamic" />
            </div>
            <div class="input-group">
                <asp:Label runat="server" Text="ZIP Code:" AssociatedControlID="txtZip" />
                <asp:TextBox runat="server" ID="txtZip" Width="200" />
                <asp:RequiredFieldValidator runat="server" ControlToValidate="txtZip" 
                    ErrorMessage="ZIP Code is required" ValidationGroup="Address" 
                    CssClass="validation-error" Display="Dynamic" />
            </div>
            <asp:Button runat="server" ID="btnProceed" Text="Proceed" OnClick="btnProceed_Click" ValidationGroup="Address" />
            <asp:Label runat="server" ID="lblAddressError" ForeColor="Red" style="display: block; margin-top: 10px;" />
        </asp:Panel>
    </div>

</form>
</body>
</html>