<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeBehind="Default.aspx.cs" Inherits="CacheAsideExample._Default" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
    <style type="text/css">
        .style1
        {
            width: 516px;
            height: 97px;
        }
    </style>
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <h2>
        <img alt="WF Logo" class="style1" src="Images/NET-WF.png" /></h2>
    <p>
        This example shows how you can use the WF4 Caching Activities with Windows Server
        AppFabric Caching
    </p>
    <h2>
        Try It</h2>
    <ol>
        <li>Using PowerShell start the AppFabric Caching Cluster</li>
        <li>Set a breakpoint in the SampleDataRepository.Get method</li>
        <li>Start Debugging</li>
        <li>Enter a number between 0 and 20 and click submit</li>
        <li>The debugger will break in SampleDataRepository.Get because the value was not found
            in the cache</li>
        <li>Enter the same value again</li>
        <li>This time the debugger will not break because the value was found in the cache</li>
    </ol>
    <p>
        Key:
        <asp:TextBox ID="TextBoxKey" runat="server"></asp:TextBox>
        &nbsp;<asp:Button ID="ButtonSubmit" runat="server" Text="Submit" 
            onclick="ButtonSubmit_Click" />
    </p>
    <p>
        Value:
        <asp:Label ID="LabelValue" runat="server"></asp:Label>
        <br />
        <asp:RadioButton ID="RadioButtonHit" runat="server" GroupName="CacheHit" Text="Cache Hit" /><br />
        <asp:RadioButton ID="RadioButtonMiss" runat="server" GroupName="CacheHit" Text="Cache Miss" />
    </p>
</asp:Content>
