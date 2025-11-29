<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="WebApplication1.Default" %>

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

        <!-- WCF: WebDownload -->
        <div id="tryitWebDownload" class="tryit-section">
            <h3>WebDownload (WCF)</h3>
            <div class="input-group">
                <label>URL:</label>
                <asp:TextBox runat="server" ID="txtUrl" Width="400" />
                <asp:RequiredFieldValidator runat="server" ControlToValidate="txtUrl" 
                    ErrorMessage="URL is required" ValidationGroup="WebDownload" 
                    CssClass="validation-error" Display="Dynamic" />
            </div>
            <asp:Button runat="server" ID="btnWebDownload" Text="Fetch" OnClick="btnWebDownload_Click" ValidationGroup="WebDownload" />
            <pre><asp:Literal runat="server" ID="litWebDownloadResult" /></pre>
        </div>

        <!-- Word Filter -->
        <div id="tryitWordFilter" class="tryit-section">
            <h3>Word Filter (REST)</h3>
            <div class="input-group">
                <label>Text to filter:</label>
                <asp:TextBox runat="server" ID="txtWordFilterInput" TextMode="MultiLine" Rows="3" Width="400" />
                <asp:RequiredFieldValidator runat="server" ControlToValidate="txtWordFilterInput" 
                    ErrorMessage="Text is required" ValidationGroup="WordFilter" 
                    CssClass="validation-error" Display="Dynamic" />
            </div>
            <asp:Button runat="server" ID="btnWordFilter" Text="Filter" OnClick="btnWordFilter_Click" ValidationGroup="WordFilter" />
            <pre><asp:Literal runat="server" ID="litWordFilterResult" /></pre>
        </div>

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

        <!-- Catalog List All -->
        <div id="tryitCatalogList" class="tryit-section">
            <h3>Catalog List All (REST)</h3>
            <div class="input-group">
                <label>Click to list all items in the catalog:</label>
            </div>
            <asp:Button runat="server" ID="btnCatalogList" Text="List All" OnClick="btnCatalogList_Click" />
            <pre><asp:Literal runat="server" ID="litCatalogListResult" /></pre>
        </div>

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

        <!-- DLL Encryption -->
        <div id="tryitDllEncrypt" class="tryit-section">
            <h3>Encryption (DLL)</h3>
            <div class="input-group">
                <label>Data to encrypt:</label>
                <asp:TextBox runat="server" ID="txtDllEncryptInput" Width="400" />
                <asp:RequiredFieldValidator runat="server" ControlToValidate="txtDllEncryptInput" 
                    ErrorMessage="Data is required" ValidationGroup="DllEncrypt" 
                    CssClass="validation-error" Display="Dynamic" />
            </div>
            <asp:Button runat="server" ID="btnDllEncrypt" Text="Encrypt (DLL)" OnClick="btnDllEncrypt_Click" ValidationGroup="DllEncrypt" />
            <pre><asp:Literal runat="server" ID="litDllEncryptResult" /></pre>
        </div>

        <!-- DLL Decryption -->
        <div id="tryitDllDecrypt" class="tryit-section">
            <h3>Decryption (DLL)</h3>
            <div class="input-group">
                <label>Data to decrypt:</label>
                <asp:TextBox runat="server" ID="txtDllDecryptInput" Width="400" />
                <asp:RequiredFieldValidator runat="server" ControlToValidate="txtDllDecryptInput" 
                    ErrorMessage="Data is required" ValidationGroup="DllDecrypt" 
                    CssClass="validation-error" Display="Dynamic" />
            </div>
            <asp:Button runat="server" ID="btnDllDecrypt" Text="Decrypt (DLL)" OnClick="btnDllDecrypt_Click" ValidationGroup="DllDecrypt" />
            <pre><asp:Literal runat="server" ID="litDllDecryptResult" /></pre>
        </div>
    </section>
</form>
</body>
</html>
