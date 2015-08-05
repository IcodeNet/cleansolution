<%@ Page Language="C#" AutoEventWireup="true"  MasterPageFile="~/Site.master" CodeBehind="HireApproval.aspx.cs" Inherits="HRApplicationServices.HireApproval" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
<title>Job Applicant Approval</title>
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
<h1>Job Applicant Approval</h1>
    <table class="style2">
        <tr>
            <td class="style3">
                <asp:Label ID="Label1" runat="server" Text="Application ID"></asp:Label>
                :
            </td>
            <td>
                <asp:Label ID="LabelAppID" runat="server" Text="(ID)"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="style3">
                Name: 
            </td>
            <td>
                <asp:Label ID="LabelName" runat="server" Text="(Name)"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="style3">
               Education:
            </td>
            <td>
                <asp:Label ID="LabelEducation" runat="server" Text="(Qualifications)"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="style3">
                &nbsp;</td>
            <td>
                &nbsp;</td>
        </tr>
        <tr>
            <td class="style3">
                <asp:Button ID="ButtonHire" runat="server" Font-Bold="True" Font-Size="Large" 
                    onclick="ButtonHireClick" Text="Hire" />
            </td>
            <td>
                <asp:Button ID="ButtonNoHire" runat="server" Font-Bold="True" Font-Size="Large" 
                    onclick="ButtonNoHireClick" Text="No Hire" />
            </td>
        </tr>
    </table>
</asp:Content>
