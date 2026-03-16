<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Default.aspx.vb" Inherits="_Default" MasterPageFile="~/Site.Master" MaintainScrollPositionOnPostback="true" Debug="true" Title="Home Page" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="page-heading">
        <h3>Home Page</h3>
    </div>
    <div class="page-content">
        <section class="col-12" runat="server" id="secDefault">
            <div class="col-12">
                <br />
                <br />
                <br />
                <br />
                <br />
                <br />
                <br />
                <br />
                <br />
                <br />
                <br />
                <br />
                <br />
                <br />
                <br />
            </div>
        </section>
        <section class="row" runat="server" id="secNewsletter">
            <div class="col-12">
                <asp:Image runat="server" CssClass="w-100" ID="imgNewsletter" />
                <asp:Label runat="server" ID="lblError"></asp:Label>
            </div>
        </section>
    </div>
</asp:Content>