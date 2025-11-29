<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GeoResolve_TryIt.aspx.cs" Inherits="WebApplication1.TryIt.GeoResolve_TryIt" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <meta charset="utf-8" />
    <title>GeoResolve – TryIt</title>
</head>
<body>
    <form id="form1" runat="server">
        <h1>GeoResolve – TryIt</h1>

        <p>
            <b>Endpoint:</b>
            <asp:Label ID="lblEndpoint" runat="server" /><br />
            <b>Method:</b> POST <code>/api/threat/georesolve</code><br />
            <b>Request:</b> <code>{ location_Text: string, origin?: string }</code><br />
            <b>Response:</b> <code>{ lat: number, lon: number, r_km: number }</code>
        </p>

        <h3>Input</h3>
        <table>
            <tr>
                <td>location_Text:</td>
                <td>
                    <asp:TextBox ID="txtLocation" runat="server" Width="400"
                                 Placeholder="e.g., NW of Bila Tserkva or Vinnytsia" />
                </td>
            </tr>
            <tr>
                <td>origin (optional):</td>
                <td>
                    <asp:TextBox ID="txtOrigin" runat="server" Width="400"
                                 Placeholder="e.g., Kyiv or 50.45,30.52" />
                </td>
            </tr>
        </table>

        <br />
        <asp:Button ID="btnExampleDir" runat="server"
                    Text="Load directional example"
                    OnClick="btnExampleDir_Click" />
        <asp:Button ID="btnExampleCity" runat="server"
                    Text="Load city example"
                    OnClick="btnExampleCity_Click" />
        <asp:Button ID="btnClear" runat="server"
                    Text="Clear"
                    OnClick="btnClear_Click" />

        <br /><br />
        <asp:Button ID="btnInvoke" runat="server"
                    Text="Invoke GeoResolve"
                    OnClick="btnInvoke_Click" />

        <br /><br />
        <asp:Label ID="lblError" runat="server" ForeColor="Red" />

        <h3>Raw JSON response</h3>
        <asp:TextBox ID="txtRawJson" runat="server" TextMode="MultiLine"
                     Rows="8" Columns="100" ReadOnly="true" />

        <h3>Parsed Result</h3>
        <asp:FormView ID="fvResult" runat="server">
            <ItemTemplate>
                <table>
                    <tr>
                        <td>Latitude:</td>
                        <td><%# Eval("Lat") %></td>
                    </tr>
                    <tr>
                        <td>Longitude:</td>
                        <td><%# Eval("Lon") %></td>
                    </tr>
                    <tr>
                        <td>Radius (km):</td>
                        <td><%# Eval("RadiusKm") %></td>
                    </tr>
                </table>
            </ItemTemplate>
        </asp:FormView>
    </form>
</body>
</html>
