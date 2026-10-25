<%@ Page Language="VB" AutoEventWireup="false" CodeFile="ChangeStatus.aspx.vb" Inherits="Setting_Specification_Product_ChangeStatus" MasterPageFile="~/Site.master" MaintainScrollPositionOnPostback="true" Debug="true" Title="Product Status Change" %>

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
            <div class="col-12 col-sm-12 col-lg-8">
                <div class="card">
                    <div class="card-header">
                        <h4 class="card-title">Change Status Form</h4>
                    </div>
                    <div class="card-body">
                        <div class="form form-vertical">
                            <div class="form-body">
                                <asp:UpdatePanel ID="updateData" runat="server" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <div class="row">
                                            <div class="col-12 col-sm-12 col-lg-4 form-group">
                                                <label class="form-label">Design Type</label>
                                                <asp:DropDownList runat="server" ID="ddlDesignType" CssClass="choices form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlDesignType_SelectedIndexChanged"></asp:DropDownList>
                                            </div>
                                            <div class="col-12 col-sm-12 col-lg-8 form-group">
                                                <label class="form-label">Blind Type</label>
                                                <asp:DropDownList runat="server" ID="ddlBlindType" CssClass="choices form-select"></asp:DropDownList>
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="col-12 col-sm-12 col-lg-4 form-group">
                                                <label class="form-label">Tube Type</label>
                                                <asp:DropDownList runat="server" ID="ddlTubeType" CssClass="choices form-select"></asp:DropDownList>
                                            </div>
                                            <div class="col-12 col-sm-12 col-lg-4 form-group">
                                                <label class="form-label">Control Type</label>
                                                <asp:DropDownList runat="server" ID="ddlControlType" CssClass="choices form-select"></asp:DropDownList>
                                            </div>
                                            <div class="col-12 col-sm-12 col-lg-4 form-group">
                                                <label class="form-label">Colour Type</label>
                                                <asp:DropDownList runat="server" ID="ddlColourType" CssClass="choices form-select"></asp:DropDownList>
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="col-12">
                                                <div class="divider">
                                                    <div class="divider-text">NEW STATUS PRODUCT</div>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="col-12 col-sm-12 col-lg-4 form-group">
                                                <label class="form-label">Status</label>
                                                <asp:DropDownList runat="server" ID="ddlStatus" CssClass="choices form-select">
                                                    <asp:ListItem Value="" Text=""></asp:ListItem>
                                                    <asp:ListItem Value="In Stock" Text="In Stock"></asp:ListItem>
                                                    <asp:ListItem Value="Limited Stock" Text="Limited Stock"></asp:ListItem>
                                                    <asp:ListItem Value="Out of Stock" Text="Out of Stock"></asp:ListItem>
                                                    <asp:ListItem Value="Discontinued" Text="Discontinued"></asp:ListItem>
                                                </asp:DropDownList>
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
            <div class="col-12 col-sm-12 col-lg-5"></div>
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