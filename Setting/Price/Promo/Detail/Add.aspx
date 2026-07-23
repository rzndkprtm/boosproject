<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Add.aspx.vb" Inherits="Setting_Price_Promo_Detail_Add" MasterPageFile="~/Site.Master" MaintainScrollPositionOnPostback="true" Debug="true" Title="Add Price Promo Detail" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="page-heading">
        <div class="page-title">
            <div class="row">
                <div class="col-12 col-md-6 order-md-1 order-last">
                    <h3><%: Page.Title %></h3>
                    <p class="text-subtitle text-muted"></p>
                </div>
                <div class="col-12 col-md-6 order-md-2 order-first">
                    <nav aria-label="breadcrumb" class="breadcrumb-header float-start float-lg-end">
                        <ol class="breadcrumb">
                            <li class="breadcrumb-item"><a runat="server" href="~/">Home</a></li>
                            <li class="breadcrumb-item"><a runat="server" href="~/setting">Setting</a></li>
                            <li class="breadcrumb-item"><a runat="server" href="~/setting/price">Price</a></li>
                            <li class="breadcrumb-item"><a runat="server" href="~/setting/price/promo">Promo</a></li>
                            <li class="breadcrumb-item active" aria-current="page"><%: Page.Title %></li>
                        </ol>
                    </nav>
                </div>
            </div>
        </div>
    </div>
    <div class="page-content">
        <section class="row">
            <div class="col-12">
                <asp:UpdatePanel ID="updateData" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <div class="card">
                            <div class="card-header">
                                <h4 class="card-title">Promo Detail Form</h4>
                            </div>
                            <div class="card-body">
                                <div class="row" runat="server" id="divError">
                                    <div class="col-12">
                                        <div class="alert alert-danger">
                                            <span runat="server" id="msgError"></span>
                                        </div>
                                    </div>
                                </div>
                                <div class="row mb-3">
                                    <div class="col-12">
                                        <div class="form-group">
                                            <label class="form-label">Promo Name</label>
                                            <asp:DropDownList runat="server" ID="ddlPromo" CssClass="choices form-select" Enabled="false"></asp:DropDownList>
                                        </div>
                                    </div>
                                </div>
                                <table class="table table-bordered">
                                    <thead>
                                        <tr>
                                            <th style="width:220px;">Type</th>
                                            <th>Data</th>
                                            <th style="width:180px;">Discount</th>
                                            <th style="width:100px;"></th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <asp:Repeater ID="rptPromo" runat="server" OnItemDataBound="rptPromo_ItemDataBound" OnItemCommand="rptPromo_ItemCommand">
                                            <ItemTemplate>
                                                <tr>
                                                    <td>
                                                        <asp:DropDownList ID="ddlType" runat="server" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlType_SelectedIndexChanged">
                                                            <asp:ListItem Value=""></asp:ListItem>
                                                            <asp:ListItem Value="Designs">Design Type</asp:ListItem>
                                                            <asp:ListItem Value="Blinds">Blind Type</asp:ListItem>
                                                            <asp:ListItem Value="Products">Product</asp:ListItem>
                                                            <asp:ListItem Value="Fabrics">Fabric Type</asp:ListItem>
                                                            <asp:ListItem Value="FabricColours">Fabric Colour</asp:ListItem>
                                                            <asp:ListItem Value="FrameColours">Frame Colour</asp:ListItem>
                                                        </asp:DropDownList>
                                                    </td>
                                                    <td>
                                                        <asp:DropDownList  ID="ddlData" runat="server" CssClass="form-select"></asp:DropDownList>
                                                    </td>
                                                    <td>
                                                        <div class="input-group">
                                                            <asp:TextBox ID="txtDiscount" runat="server" CssClass="form-control" Text='<%# Eval("Discount") %>'></asp:TextBox>
                                                            <span class="input-group-text">%</span>
                                                        </div>
                                                    </td>
                                                    <td class="text-center">
                                                        <asp:LinkButton ID="btnDelete" runat="server" CssClass="btn btn-danger btn-sm" CommandName="DeleteRow" CommandArgument='<%# Container.ItemIndex %>'>Delete</asp:LinkButton>
                                                    </td>
                                                </tr>
                                            </ItemTemplate>
                                        </asp:Repeater>
                                    </tbody>
                                </table>
                                <div class="row mb-3">
                                    <div class="col-lg-12 text-end">
                                        <asp:Button ID="btnAdd" runat="server" Text="+ Add Row" CssClass="btn btn-success" OnClick="btnAdd_Click" />
                                    </div>
                                </div>
                            </div>
                            <div class="card-footer">
                                <asp:Button runat="server" ID="btnSubmit" CssClass="btn btn-primary" Text="Submit" OnClick="btnSubmit_Click" />
                                <asp:Button runat="server" ID="btnCancel" CssClass="btn btn-danger" Text="Cancel" OnClick="btnCancel_Click" />
                            </div>
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
        </section>
    </div>

    <div runat="server" visible="false">
        <asp:Label runat="server" ID="lblId"></asp:Label>
    </div>

    <div id="loadingOverlay" style="display:none; position:fixed; top:0; left:0; width:100%; height:100%; background:rgba(255,255,255,.5); z-index:99999;">
        <div class="position-absolute top-50 start-50 translate-middle">
            <div class="card shadow">
                <div class="card-body text-center">
                    <div class="spinner-border"></div>
                    <div class="mt-2">Loading...</div>
                </div>
            </div>
        </div>
    </div>

    <script type="text/javascript">
        window.addEventListener("pageshow", function () {
        var loading = document.getElementById("loadingOverlay");
            if (loading) loading.style.display = "none";
        });
        function initUpdatePanelLoading() {
            if (typeof Sys === "undefined") return;
            var prm = Sys.WebForms.PageRequestManager.getInstance();
            prm.add_beginRequest(function () {
                var loading = document.getElementById("loadingOverlay");
                if (loading) loading.style.display = "block";
            });
            prm.add_endRequest(function () {
                var loading = document.getElementById("loadingOverlay");
                if (loading) loading.style.display = "none";
            });
        }
        document.addEventListener("DOMContentLoaded", function () {
            initUpdatePanelLoading();
        });
        window.history.replaceState(null, null, window.location.href);
    </script>
</asp:Content>