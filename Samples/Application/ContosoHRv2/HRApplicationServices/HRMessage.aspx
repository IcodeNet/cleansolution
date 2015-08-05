<%@ Page Language="C#" AutoEventWireup="true"  MasterPageFile="~/Site.master"  CodeBehind="HRMessage.aspx.cs" Inherits="HRApplicationServices.HRMessage" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
        <title>Contoso HR Web Error</title>
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
<h1>Message</h1>
    <asp:Label ID="LabelError" runat="server" 
        Text="We're sorry, an error has occured and has been logged, please try again or contact support"></asp:Label>
</asp:Content>
