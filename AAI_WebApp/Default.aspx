<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="AAI_WebApp.Default" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>Air-Alert Intelligence (AAI) – Service Directory</title>
</head>
<body>
    <form id="form1" runat="server">
        <h1>Air-Alert Intelligence (AAI)</h1>

        <p>
            This application ingests public alert posts (Telegram channels and official sites),
            normalizes them, geo-resolves locations, and exposes services for content analysis
            and risk estimation. This page lists all available components and provides TryIt
            interfaces to test them.
        </p>

        <asp:Button ID="btnMember" runat="server" Text="Member Page"
            PostBackUrl="~/Member.aspx" />
        <asp:Button ID="btnStaff" runat="server" Text="Staff Page"
            PostBackUrl="~/Staff.aspx" />

        <h2>Service Directory</h2>
        <asp:GridView ID="gvDirectory" runat="server" AutoGenerateColumns="false">
            <Columns>
                <asp:BoundField HeaderText="Provider" DataField="Provider" />
                <asp:BoundField HeaderText="Type" DataField="Type" />
                <asp:BoundField HeaderText="Name" DataField="Name" />
                <asp:BoundField HeaderText="Parameters" DataField="Parameters" />
                <asp:BoundField HeaderText="Return" DataField="Return" />
                <asp:BoundField HeaderText="Description" DataField="Description" />
                <asp:TemplateField HeaderText="TryIt">
                    <ItemTemplate>
                        <asp:HyperLink runat="server"
                            Text="Open"
                            NavigateUrl='<%# Eval("TryItUrl") %>' />
                    </ItemTemplate>
                </asp:TemplateField>

            </Columns>
        </asp:GridView>

        <h2>Quick Try – Hash Function (Local DLL)</h2>
        <asp:TextBox ID="txtHashInput" runat="server" Width="400" />
        <asp:Button ID="btnHash" runat="server" Text="Hash" OnClick="btnHash_Click" />
        <br />
        <asp:Label ID="lblHashOutput" runat="server" />

        <h2>Quick Try – Active Sessions (Global.asax)</h2>
        <asp:Button ID="btnSessions" runat="server" Text="Refresh" OnClick="btnSessions_Click" />
        <asp:Label ID="lblSessions" runat="server" />
    </form>
</body>
</html>
