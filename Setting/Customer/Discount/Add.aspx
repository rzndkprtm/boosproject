<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Add.aspx.vb" Inherits="Setting_Customer_Discount_Add" MasterPageFile="~/Site.master" MaintainScrollPositionOnPostback="true" Debug="true" Title="Add Customer Discount" %>

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
                            <li class="breadcrumb-item"><a runat="server" href="~/setting/customer">Customer</a></li>
                            <li class="breadcrumb-item"><a runat="server" href="~/setting/customer/discount">Discount</a></li>
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
                <div class="card">
                    <div class="card-header">
                        <h4 class="card-title">Discount Form</h4>
                    </div>
                    <div class="card-body">
                        <div class="row mt-3" runat="server" id="divError">
                            <div class="col-12">
                                <div class="alert alert-danger">
                                    <span runat="server" id="msgError"></span>
                                </div>
                            </div>
                        </div>
                        <asp:UpdatePanel ID="updateData" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <div class="row mb-3">
                                    <div class="col-12 col-sm-12 col-lg-7 form-group">
                                        <label class="form-label">Account</label>
                                        <asp:DropDownList runat="server" ID="ddlCustomer" CssClass="choices form-select"></asp:DropDownList>
                                    </div>
                                    <div class="col-12 col-sm-12 col-lg-5 form-group">
                                        <label class="form-label">Type</label>
                                        <asp:DropDownList runat="server" ID="ddlType" CssClass="choices form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlType_SelectedIndexChanged">
                                            <asp:ListItem Value="" Text=""></asp:ListItem>
                                            <asp:ListItem Value="Designs" Text="Product"></asp:ListItem>
                                            <asp:ListItem Value="PriceProductGroups" Text="Product Group"></asp:ListItem>
                                            <asp:ListItem Value="RollerFabrics" Text="Fabric Type (Roller)"></asp:ListItem>
                                            <asp:ListItem Value="RollerFabricColours" Text="Fabric Colour (Roller)"></asp:ListItem>
                                            <asp:ListItem Value="RomanFabrics" Text="Fabric Type (Roman)"></asp:ListItem>
                                            <asp:ListItem Value="RomanFabricColours" Text="Fabric Colour (Roman)"></asp:ListItem>
                                            <asp:ListItem Value="PanelGlideFabrics" Text="Fabric Type (Panel Glide)"></asp:ListItem>
                                            <asp:ListItem Value="PanelGlideFabricColours" Text="Fabric Colour (Panel Glide)"></asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                </div>
                                <table class="table table-bordered">
                                    <thead>
                                        <tr>
                                            <th style="width:600px;">Product</th>
                                            <th style="width:150px;">Discount (%)</th>
                                            <th>Description</th>
                                            <th style="width:80px;"></th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <asp:Repeater ID="rptDiscount" runat="server" OnItemDataBound="rptDiscount_ItemDataBound" OnItemCommand="rptDiscount_ItemCommand">
                                            <ItemTemplate>
                                                <tr>
                                                    <td>
                                                        <asp:DropDownList runat="server" ID="ddlProduct" CssClass="choices form-select"></asp:DropDownList>
                                                    </td>
                                                    <td>
                                                        <asp:TextBox runat="server" ID="txtDiscount" CssClass="form-control" placeholder="Discount ......" autocomplete="off"></asp:TextBox>
                                                    </td>
                                                    <td>
                                                        <asp:TextBox runat="server" ID="txtDescription" CssClass="form-control" placeholder="Description ..." autocomplete="off"></asp:TextBox>
                                                    </td>
                                                    <td class="text-center">
                                                        <asp:LinkButton runat="server" ID="btnDelete" CssClass="btn btn-danger btn-sm" CommandName="DeleteRow" CommandArgument='<%# Container.ItemIndex %>'>Delete</asp:LinkButton>
                                                    </td>
                                                </tr>
                                            </ItemTemplate>
                                        </asp:Repeater>
                                    </tbody>
                                </table>
                                <div class="row mb-3">
                                    <div class="col-lg-12 text-start">
                                        <asp:Button ID="btnAdd" runat="server" Text="+ Add Row" CssClass="btn btn-success" OnClick="btnAdd_Click" />
                                    </div>
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                    <div class="card-footer">
                        <asp:Button runat="server" ID="btnSubmit" CssClass="btn btn-primary" Text="Submit" OnClick="btnSubmit_Click" />
                        <asp:Button runat="server" ID="btnCancel" CssClass="btn btn-danger" Text="Cancel" OnClick="btnCancel_Click" />
                    </div>
                </div>
            </div>
        </section>
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

    <div runat="server" visible="false">
        <asp:Label runat="server" ID="lblCustomerId"></asp:Label>
        <asp:Label runat="server" ID="lblReturnPage"></asp:Label>        
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
                initChoices();
            });
        }
        function initChoices() {
            document.querySelectorAll("select.choices").forEach(function (el) {
                if (el.choices) {
                    el.choices.destroy();
                }
                el.choices = new Choices(el, {
                    searchEnabled: true,
                    itemSelectText: '',
                    shouldSort: false,
                    searchResultLimit: 50
                });
            });
        }
        document.addEventListener("DOMContentLoaded", function () {
            initUpdatePanelLoading();
            initChoices();
        });
        window.history.replaceState(null, null, window.location.href);
    </script>
</asp:Content>
