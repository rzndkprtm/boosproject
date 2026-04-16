<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Default.aspx.vb" Inherits="_Default" MasterPageFile="~/Site.Master" MaintainScrollPositionOnPostback="true" Debug="true" Title="Home Page" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="page-heading">
        <h3>Home Page</h3>
    </div>
    <div class="page-content">
        <section class="row" runat="server" id="secDefault">
            <div class="col-12">
                <div class="card">
                    <div class="card-content">
                        <div class="card-header">
                            <h3 class="card-title">Newsletter</h3>
                        </div>
                    </div>
                    <div class="card-body">
                        <asp:Image runat="server" CssClass="w-100" ID="imgNewsletter" />
                    </div>
                </div>
            </div>
        </section>
    </div>
</asp:Content>