<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Add.aspx.vb" Inherits="Setting_Price_Base_Add" MasterPageFile="~/Site.Master" MaintainScrollPositionOnPostback="true" Debug="true" Title="Add Price Base" %>

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
                            <li class="breadcrumb-item"><a runat="server" href="~/setting/price/base">Base</a></li>
                            <li class="breadcrumb-item active" aria-current="page"><%: Page.Title %></li>
                        </ol>
                    </nav>
                </div>
            </div>
        </div>
    </div>
    <div class="page-content">
        <section class="row">
            <div class="col-12 col-sm-12 col-lg-7">
                <div class="card">
                    <div class="card-header">
                        <h4 class="card-title">Price Base Form</h4>
                    </div>
                    <div class="card-body">
                        <div class="form form-vertical">
                            <div class="form-body">
                                <asp:UpdatePanel ID="updateData" runat="server" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <div class="row mb-2">
                                            <div class="col-12 col-sm-12 col-lg-6 form-group">
                                                <label class="form-label">Category</label>
                                                <asp:DropDownList runat="server" ID="ddlCategory" CssClass="choices form-select">
                                                    <asp:ListItem Value="" Text=""></asp:ListItem>
                                                    <asp:ListItem Value="Sell" Text="Sell Price"></asp:ListItem>
                                                    <asp:ListItem Value="Buy" Text="Buy Price"></asp:ListItem>
                                                </asp:DropDownList>
                                            </div>
                                            <div class="col-12 col-sm-12 col-lg-6 form-group">
                                                <label class="form-label">Method</label>
                                                <asp:DropDownList runat="server" ID="ddlMethod" CssClass="choices form-select">
                                                    <asp:ListItem Value="" Text=""></asp:ListItem>
                                                    <asp:ListItem Value="Cost" Text="Cost"></asp:ListItem>
                                                    <asp:ListItem Value="Square Metre" Text="Square Metre"></asp:ListItem>
                                                    <asp:ListItem Value="Linear Metre" Text="Linear Metre"></asp:ListItem>
                                                </asp:DropDownList>
                                            </div>
                                        </div>
                                        <div class="row mb-2">
                                            <div class="col-12 form-group">
                                                <label class="form-label">Price Group</label>
                                                <asp:DropDownList runat="server" ID="ddlPriceGroup" CssClass="choices form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlPriceGroup_SelectedIndexChanged"></asp:DropDownList>
                                            </div>
                                        </div>
                                        <div class="row mb-2">
                                            <div class="col-12 form-group">
                                                <label class="form-label">Product Group</label>
                                                <asp:DropDownList runat="server" ID="ddlProductGroup" CssClass="choices form-select"></asp:DropDownList>
                                            </div>
                                        </div>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                                <div class="row mb-2">
                                    <div class="col-12 col-sm-12 col-lg-6 form-group">
                                        <label class="form-label">Height</label>
                                        <asp:TextBox runat="server" ID="txtHeight" CssClass="form-control" placeholder="Height ..."></asp:TextBox>
                                    </div>
                                    <div class="col-12 col-sm-12 col-lg-6 form-group">
                                        <label class="form-label">Width</label>
                                        <asp:TextBox runat="server" ID="txtWidth" CssClass="form-control" placeholder="Width ..."></asp:TextBox>
                                    </div>
                                </div>
                                <div class="row mb-2">
                                    <div class="col-12 col-sm-12 col-lg-6 form-group">
                                        <label class="form-label">Price</label>
                                        <asp:TextBox runat="server" ID="txtPrice" CssClass="form-control" placeholder="Price ..." autocomplete="off"></asp:TextBox>
                                    </div>
                                    <div class="col-12 col-sm-12 col-lg-6 form-group" runat="server" id="divConditional">
                                        <label class="form-label">Conditional</label>
                                        <asp:TextBox runat="server" ID="txtConditional" CssClass="form-control" placeholder="Conditional ..." autocomplete="off"></asp:TextBox>
                                    </div>
                                </div>
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
                    <div class="card-footer text-start">
                        <asp:Button runat="server" ID="btnSubmitAdd" CssClass="btn btn-primary" Text="Submit & Add Another" OnClick="btnSubmitAdd_Click" />
                        <asp:Button runat="server" ID="btnSubmitFinish" CssClass="btn btn-secondary me-2" Text="Submit & Finish" OnClick="btnSubmitFinish_Click" />
                        <asp:Button runat="server" ID="btnCancel" CssClass="btn btn-danger" Text="Cancel" OnClick="btnCancel_Click" />
                    </div>
                </div>
            </div>
            <div class="col-12 col-sm-12 col-lg-5">
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
