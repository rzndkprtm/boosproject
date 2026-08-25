<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Add.aspx.vb" Inherits="Setting_Specification_Product_Add" MasterPageFile="~/Site.Master" MaintainScrollPositionOnPostback="true" Debug="true" Title="Add Product" %>

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
                            <li class="breadcrumb-item"><a runat="server" href="~/setting/specification">Specification</a></li>
                            <li class="breadcrumb-item"><a runat="server" href="~/setting/specification/product">Product</a></li>
                            <li class="breadcrumb-item active" aria-current="page"><%: Page.Title %></li>
                        </ol>
                    </nav>
                </div>
            </div>
        </div>
    </div>
    <div class="page-content">
        <section class="row">
            <div class="col-lg-8 col-md-12 col-sm-12">
                <div class="card">
                    <div class="card-header">
                        <h4 class="card-title">Product Form</h4>
                    </div>
                    <div class="card-body">
                        <div class="form form-vertical">
                            <div class="form-body">
                                <asp:UpdatePanel ID="updateData" runat="server" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <div class="row mb-2">
                                            <div class="col-12 col-sm-12 col-lg-5 mb-2">
                                                <div class="form-group">
                                                    <label class="form-label">Design Type</label>
                                                    <asp:DropDownList runat="server" ID="ddlDesign" CssClass="choices form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlDesign_SelectedIndexChanged"></asp:DropDownList>
                                                </div>
                                            </div>
                                            <div class="col-12 col-sm-12 col-lg-7 mb-2">
                                                <div class="form-group">
                                                    <label class="form-label">Blind Type</label>
                                                    <asp:DropDownList runat="server" ID="ddlBlind" CssClass="choices form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlBlind_SelectedIndexChanged"></asp:DropDownList>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="row mb-2">
                                            <div class="col-12">
                                                <div class="form-group">
                                                    <label class="form-label">Company Detail</label>
                                                    <asp:ListBox runat="server" ID="lbCompanyDetail" CssClass="choices form-select multiple-remove" SelectionMode="Multiple"></asp:ListBox>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="row mb-3">
                                            <div class="col-12">
                                                <div class="form-group">
                                                    <label class="form-label">Job Sheet Name</label>
                                                    <asp:DropDownList runat="server" ID="ddlJobSheet" CssClass="choices form-select"></asp:DropDownList>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="row mb-3">
                                            <div class="col-12">
                                                <div class="form-group">
                                                    <label class="form-label">Product Name</label>
                                                    <asp:TextBox runat="server" ID="txtName" CssClass="form-control" placeholder="Product Name" autocomplete="off"></asp:TextBox>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="row mb-3">
                                            <div class="col-12">
                                                <div class="form-group">
                                                    <label class="form-label">Invoice Name</label>
                                                    <asp:TextBox runat="server" ID="txtInvoiceName" CssClass="form-control" placeholder="Invoice Name" autocomplete="off"></asp:TextBox>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="row mb-3">
                                            <div class="col-12 col-sm-12 col-lg-4 mb-2">
                                                <div class="form-group">
                                                    <label class="form-label">Tube Type</label>
                                                    <asp:DropDownList runat="server" ID="ddlTube" CssClass="choices form-select"></asp:DropDownList>
                                                </div>
                                            </div>
                                            <div class="col-12 col-sm-12 col-lg-4 mb-2">
                                                <div class="form-group">
                                                    <label class="form-label">Control Type</label>
                                                    <asp:DropDownList runat="server" ID="ddlControl" CssClass="choices form-select"></asp:DropDownList>
                                                </div>
                                            </div>
                                            <div class="col-12 col-sm-12 col-lg-4 mb-2">
                                                <div class="form-group">
                                                    <label class="form-label">Colour Type</label>
                                                    <asp:DropDownList runat="server" ID="ddlColour" CssClass="choices form-select"></asp:DropDownList>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="row mb-3">
                                            <div class="col-12">
                                                <div class="form-group">
                                                    <label class="form-label">Description</label>
                                                    <asp:TextBox runat="server" TextMode="MultiLine" ID="txtDescription" Height="100px" CssClass="form-control" placeholder="Description ..." autocomplete="off" style="resize:none;"></asp:TextBox>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="row mb-3">
                                            <div class="col-12 col-sm-12 col-lg-3">
                                                <div class="form-group">
                                                    <label class="form-label">Status</label>
                                                    <asp:DropDownList runat="server" ID="ddlStatus" CssClass="form-select">
                                                        <asp:ListItem Value="" Text=""></asp:ListItem>
                                                        <asp:ListItem Value="In Stock" Text="In Stock"></asp:ListItem>
                                                        <asp:ListItem Value="Limited Stock" Text="Limited Stock"></asp:ListItem>
                                                        <asp:ListItem Value="Out of Stock" Text="Out of Stock"></asp:ListItem>
                                                        <asp:ListItem Value="Discontinued" Text="Discontinued"></asp:ListItem>
                                                    </asp:DropDownList>
                                                </div>
                                            </div>
                                        </div>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                                <div class="row" runat="server" id="divError">
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

                var isMultiple = el.multiple;

                el.choices = new Choices(el, {
                    searchEnabled: true,
                    itemSelectText: '',
                    shouldSort: false,
                    removeItemButton: isMultiple
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