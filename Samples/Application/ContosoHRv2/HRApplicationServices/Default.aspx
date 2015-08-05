<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeBehind="Default.aspx.cs" Inherits="HRApplicationServices.Default" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <h2>
        Join Our Team
    </h2>
    <p>
        Contoso offers world class opportunities for qualified applicants. Apply today!
    </p>
    <asp:Table ID="Table1" runat="server" Width="381px">
        <asp:TableRow>
            <asp:TableCell HorizontalAlign="Right">Name</asp:TableCell>
            <asp:TableCell>
                <asp:TextBox ID="txtName" runat="server" Width="300" />
            </asp:TableCell></asp:TableRow><asp:TableRow>
            <asp:TableCell HorizontalAlign="Right">email</asp:TableCell><asp:TableCell>
                <asp:TextBox ID="txtEmail" runat="server" Width="300" />
            </asp:TableCell></asp:TableRow><asp:TableRow>
            <asp:TableCell HorizontalAlign="Right">Education</asp:TableCell><asp:TableCell>
                <asp:DropDownList ID="ddEducation" runat="server" Width="300">
                    <asp:ListItem>None</asp:ListItem>
                    <asp:ListItem>Bachelors</asp:ListItem>
                    <asp:ListItem>Masters</asp:ListItem>
                    <asp:ListItem>Doctorate</asp:ListItem>
                </asp:DropDownList>
            </asp:TableCell></asp:TableRow><asp:TableRow>
            <asp:TableCell></asp:TableCell><asp:TableCell>
                <asp:Button ID="buttonApply" runat="server" Text="Apply" OnClick="ButtonApplyClick" Width="200" /></asp:TableCell>
        </asp:TableRow>
    </asp:Table>
</asp:Content>
