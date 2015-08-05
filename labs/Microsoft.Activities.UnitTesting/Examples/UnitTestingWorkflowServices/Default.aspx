<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeBehind="Default.aspx.cs" Inherits="WorkflowExtensionExample._Default" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <h2>
        What is an extension?
    </h2>
    <p>
        Extensions are objects that you can add to a Workflow Host such as WorkflowApplication
        or WorkflowServiceHost that can be used by Activities that look for them. Activities
        access the extensions collection through the <a href="http://msdn.microsoft.com/en-us/library/dd486210.aspx">
            ActivityContext.GetExtension&lt;T&gt;</a> method which means that you can have
        only one extension of a given type in the collection at a time.
    </p>
    <p>
        In this example you will see how you can use the <a href="http://msdn.microsoft.com/en-us/library/ee473669.aspx">
            Add&lt;T&gt;(Func&lt;T&gt;)</a> to allow an activity to provide a creation function
        for your extension.
    </p>
    <h3>Try It</h3>
    <p>
        <asp:TextBox ID="TextBoxNum" runat="server">123</asp:TextBox><br />
        <asp:LinkButton ID="LinkButtonTestService" runat="server" 
            onclick="LinkButtonTest_Click">Test Service</asp:LinkButton>&nbsp;or
        <asp:LinkButton ID="LinkButtonTestActivity" runat="server" 
            onclick="LinkButton1_Click">Test Activity</asp:LinkButton>
        <br />
        Server Response: <asp:Label ID="LabelResult" runat="server" />
    </p>
</asp:Content>
