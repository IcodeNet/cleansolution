<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeBehind="Default.aspx.cs" Inherits="XamlAssemblyResolutionWeb._Default" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <p>
        This sample illustrates how XAML Assembly Resolution occurs in ASP.NET and Workflow Services
    </p>
    <h3>Workflow Services XAML Resolution</h3>
    <p><asp:Button ID="buttonTestWS" runat="server" Text="Test Workflow Service" 
            onclick="buttonTestWS_Click" /></p>
    <table>
    <thead>
    <tr>
    <td>Assembly</td>
    <td>Version</td>
    <td>Path</td>
    </tr>
    </thead>
    <tr>
    <td><asp:Label ID="labelWebAssembly" runat="server" /></td>
    <td><asp:Label ID="labelWebVersion" runat="server" /></td>
    <td><asp:Label ID="labelWebPath" runat="server" /></td>    
    </tr>
    <tr>
    <td><asp:Label ID="labelActivityAssembly" runat="server" /></td>
    <td><asp:Label ID="labelActivityVersion" runat="server" /></td>
    <td><asp:Label ID="labelActivityPath" runat="server" /></td>    
    </tr>
    </table>

</asp:Content>
