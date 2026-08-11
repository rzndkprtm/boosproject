<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Edit.aspx.vb" Inherits="Setting_Customer_Discount_Edit" MasterPageFile="~/Site.master" MaintainScrollPositionOnPostback="true" Debug="true" Title="Edit Customer Discount" %>

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
            <div class="col-12 col-sm-12 col-lg-8">
                <div class="card">
                    <div class="card-header">
                        <h4 class="card-title">Discount Form</h4>
                    </div>
                    <div class="card-body">
                        <div class="form form-vertical">
                            <div class="form-body">
                                <asp:UpdatePanel ID="updateData" runat="server" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <div class="row mb-2">
                                            <div class="col-12 form-group">
                                                <label class="form-label">Account</label>
                                                <asp:DropDownList runat="server" ID="ddlCustomer" CssClass="choices form-select"></asp:DropDownList>
                                            </div>
                                        </div>
                                        <div class="row mb-2">
                                            <div class="col-12 col-sm-12 col-lg-4 form-group">
                                                <label class="form-label">Type</label>
                                                <asp:DropDownList runat="server" ID="ddlType" CssClass="choices form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlType_SelectedIndexChanged">
                                                    <asp:ListItem Value="" Text=""></asp:ListItem>
                                                    <asp:ListItem Value="Designs" Text="Product"></asp:ListItem>
                                                    <asp:ListItem Value="PriceProductGroups" Text="Product Group"></asp:ListItem>
                                                    <asp:ListItem Value="RollerFabrics" Text="Roller Fabric Type"></asp:ListItem>
                                                    <asp:ListItem Value="RollerFabricColours" Text="Roller Fabric Colour"></asp:ListItem>
                                                    <asp:ListItem Value="RomanFabrics" Text="Roman Fabric Type"></asp:ListItem>
                                                    <asp:ListItem Value="RomanFabricColours" Text="Roman Fabric Colour"></asp:ListItem>
                                                    <asp:ListItem Value="PanelGlideFabrics" Text="Panel Glide Fabric Type"></asp:ListItem>
                                                    <asp:ListItem Value="PanelGlideFabricColours" Text="Panel Glide Fabric Colour"></asp:ListItem>
                                                </asp:DropDownList>
                                            </div>
                                            <div class="col-12 col-sm-12 col-lg-8 form-group">
                                                <label class="form-label">Product</label>
                                                <asp:DropDownList runat="server" ID="ddlProduct" CssClass="choices form-select"></asp:DropDownList>
                                            </div>
                                        </div>
                                        <div class="row mb-2">
                                            <div class="col-12 col-sm-12 col-lg-4 form-group">
                                                <label class="form-label">Discount</label>
                                                <div class="input-group">
                                                    <asp:TextBox runat="server" TextMode="Number" ID="txtDiscount" CssClass="form-control" placeholder="Discount ......" autocomplete="off"></asp:TextBox>
                                                    <span class="input-group-text">%</span>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="row mb-2">
                                            <div class="col-12 form-group">
                                                <label class="form-label">Description</label>
                                                <asp:TextBox runat="server" TextMode="MultiLine" ID="txtDescription" CssClass="form-control" Height="100px" placeholder="Description ..." autocomplete="off" style="resize:none;"></asp:TextBox>
                                            </div>
                                        </div>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                                <div class="row mt-3" runat="server" id="divError">
                                    <div class="col-12">
                                        <div class="alert alert-danger">
                                            <span runat="server" id="msgError"></span>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="card-footer text-center">
                        <asp:Button runat="server" ID="btnSubmit" CssClass="btn btn-primary" Text="Submit" OnClick="btnSubmit_Click" />
                        <asp:Button runat="server" ID="btnCancel" CssClass="btn btn-danger" Text="Cancel" OnClick="btnCancel_Click" />
                    </div>
                </div>
            </div>
            <div class="col-12 col-sm-12 col-lg-4">
                <div class="card">
                    <div class="card-header">
                        <h4 class="card-title text-center">Information</h4>
                    </div>
                    <div class="card-content">
                        <div class="card-body"></div>
                    </div>
                </div>
            </div>
        </section>
    </div>

    <div runat="server" visible="false">
        <asp:Label runat="server" ID="lblId"></asp:Label>
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
                    shouldSort: false
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
