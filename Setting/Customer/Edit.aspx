<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Edit.aspx.vb" Inherits="Setting_Customer_Edit" MasterPageFile="~/Site.Master" MaintainScrollPositionOnPostback="true" Debug="true" Title="Edit Customer" %>

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
                            <li class="breadcrumb-item"><a runat="server" href="~/setting/customer/list">List</a></li>
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
                        <h4 class="card-title">Customer Form</h4>
                    </div>
                    <div class="card-content">
                        <div class="card-body">
                            <div class="form form-vertical">
                                <div class="form-body">
                                    <asp:UpdatePanel ID="updateData" runat="server" UpdateMode="Conditional">
                                        <ContentTemplate>
                                            <div class="row mb-2" runat="server" id="divDebtorCode">
                                                <div class="col-12 col-sm-12 col-lg-5 form-group">
                                                    <label class="form-label">Debtor Code</label>
                                                    <asp:TextBox runat="server" ID="txtDebtorCode" CssClass="form-control" placeholder="Debtor Code ..." autocomplete="off"></asp:TextBox>
                                                </div>
                                            </div>
                                            <div class="row mb-2">
                                                <div class="col-12 form-group">
                                                    <label class="form-label">Customer Name</label>
                                                    <asp:TextBox runat="server" ID="txtName" CssClass="form-control" placeholder="Customer Name ..." autocomplete="off"></asp:TextBox>
                                                </div>
                                            </div>
                                            <div class="row mb-2" runat="server" id="divLevelSponsor">
                                                <div class="col-12 col-sm-12 col-lg-4 mb-2 form-group">
                                                    <label class="form-label">Level</label>
                                                    <asp:DropDownList runat="server" ID="ddlLevel" CssClass="choices form-select">
                                                        <asp:ListItem Value="" Text=""></asp:ListItem>
                                                        <asp:ListItem Value="Master" Text="Master Customer"></asp:ListItem>
                                                        <asp:ListItem Value="Standard" Text="Standard Customer"></asp:ListItem>
                                                        <asp:ListItem Value="Primary" Text="Primary Customer"></asp:ListItem>
                                                        <asp:ListItem Value="Linked" Text="Linked Customer"></asp:ListItem>
                                                    </asp:DropDownList>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-8 mb-2 form-group">
                                                    <label class="form-label">Linked Customer to</label>
                                                    <asp:DropDownList runat="server" ID="ddlPrimary" CssClass="choices form-select"></asp:DropDownList>
                                                </div>
                                            </div>
                                            <div class="row mb-2" runat="server" id="divCompany">
                                                <div class="col-12 col-sm-12 col-lg-6 mb-2 form-group">
                                                    <label class="form-label">Company</label>
                                                    <asp:DropDownList runat="server" ID="ddlCompany" CssClass="choices form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlCompany_SelectedIndexChanged"></asp:DropDownList>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-6 mb-2 form-group">
                                                    <label class="form-label">Company Detail</label>
                                                    <asp:DropDownList runat="server" ID="ddlCompanyDetail" CssClass="choices form-select"></asp:DropDownList>
                                                </div>
                                            </div>
                                            <div class="row mb-2" runat="server" id="divAreaOperator">
                                                <div class="col-5 col-sm-12 col-lg-6 mb-2 form-group">
                                                    <label class="form-label">Area</label>
                                                    <asp:DropDownList runat="server" ID="ddlArea" CssClass="choices form-select">
                                                        <asp:ListItem Value="" Text=""></asp:ListItem>
                                                        <asp:ListItem Value="NSW" Text="NSW"></asp:ListItem>
                                                        <asp:ListItem Value="NSW 1" Text="NSW 1"></asp:ListItem>
                                                        <asp:ListItem Value="NSW 2" Text="NSW 2"></asp:ListItem>
                                                        <asp:ListItem Value="QLD" Text="QLD"></asp:ListItem>
                                                        <asp:ListItem Value="SA" Text="SA"></asp:ListItem>
                                                        <asp:ListItem Value="TAS" Text="TAS"></asp:ListItem>
                                                        <asp:ListItem Value="VIC" Text="VIC"></asp:ListItem>
                                                        <asp:ListItem Value="VIC 1" Text="VIC 1"></asp:ListItem>
                                                        <asp:ListItem Value="VIC 2" Text="VIC 2"></asp:ListItem>
                                                        <asp:ListItem Value="WA" Text="WA"></asp:ListItem>
                                                        <asp:ListItem Value="JKT" Text="JKT"></asp:ListItem>
                                                    </asp:DropDownList>
                                                </div>
                                                <div class="col-7 col-sm-12 col-lg-6 mb-2 form-group">
                                                    <label class="form-label">Operator</label>
                                                    <asp:ListBox runat="server" ID="lbOperator" CssClass="choices form-select multiple-remove" SelectionMode="Multiple"></asp:ListBox>
                                                </div>
                                            </div>
                                            <div class="row mb-3">
                                                <div class="col-12 col-sm-12 col-lg-4 mb-2 form-group">
                                                    <label class="form-label">Price Group</label>
                                                    <asp:DropDownList runat="server" ID="ddlPriceGroup" CssClass="choices form-select"></asp:DropDownList>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-4 mb-2 form-group">
                                                    <label class="form-label">Shutter Price Group</label>
                                                    <asp:DropDownList runat="server" ID="ddlPriceGroupShutter" CssClass="choices form-select"></asp:DropDownList>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-4 mb-2 form-group">
                                                    <label class="form-label">Door Price Group</label>
                                                    <asp:DropDownList runat="server" ID="ddlPriceGroupDoor" CssClass="choices form-select"></asp:DropDownList>
                                                </div>
                                            </div>
                                            <div class="row mb-2">
                                                <div class="col-6 col-sm-12 col-lg-3 mb-2 form-group">
                                                    <label class="form-label">On Stop</label>
                                                    <asp:DropDownList runat="server" ID="ddlOnStop" CssClass="form-select">
                                                        <asp:ListItem Value="0" Text="No"></asp:ListItem>
                                                        <asp:ListItem Value="1" Text="Yes"></asp:ListItem>
                                                    </asp:DropDownList>
                                                </div>
                                                <div class="col-6 col-sm-12 col-lg-3 mb-2 form-group">
                                                    <label class="form-label">Cash Sale</label>
                                                    <asp:DropDownList runat="server" ID="ddlCashSale" CssClass="form-select">
                                                        <asp:ListItem Value="1" Text="Yes"></asp:ListItem>
                                                        <asp:ListItem Value="0" Text="No"></asp:ListItem>
                                                    </asp:DropDownList>
                                                </div>
                                                <div class="col-6 col-sm-12 col-lg-3 mb-2 form-group">
                                                    <label class="form-label">Newsletter</label>
                                                    <asp:DropDownList runat="server" ID="ddlNewsletter" CssClass="form-select">
                                                        <asp:ListItem Value="1" Text="Yes"></asp:ListItem>
                                                        <asp:ListItem Value="0" Text="No"></asp:ListItem>
                                                    </asp:DropDownList>
                                                </div>
                                                <div class="col-6 col-sm-12 col-lg-3 mb-2 form-group">
                                                    <label class="form-label">Minimum Surcharge</label>
                                                    <asp:DropDownList runat="server" ID="ddlMinSurcharge" CssClass="form-select">
                                                        <asp:ListItem Value="1" Text="Yes"></asp:ListItem>
                                                        <asp:ListItem Value="0" Text="No"></asp:ListItem>
                                                    </asp:DropDownList>
                                                </div>
                                            </div>
                                            <div class="row mb-2">
                                                <div class="col-12 col-sm-12 col-lg-3 form-group">
                                                    <label class="form-label">Active</label>
                                                    <asp:DropDownList runat="server" ID="ddlActive" CssClass="form-select">
                                                        <asp:ListItem Value="1" Text="Yes"></asp:ListItem>
                                                        <asp:ListItem Value="0" Text="No"></asp:ListItem>
                                                    </asp:DropDownList>
                                                </div>
                                            </div>
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                    <div class="row mb-2" runat="server" id="divError">
                                        <div class="col-12">
                                            <div class="alert alert-danger">
                                                <span runat="server" id="msgError"></span>
                                            </div>
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

    <div runat="server" visible="false">
        <asp:Label runat="server" ID="lblId"></asp:Label>
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
